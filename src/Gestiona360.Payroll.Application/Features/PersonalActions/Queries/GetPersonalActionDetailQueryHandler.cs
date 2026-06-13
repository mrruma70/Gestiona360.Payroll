using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Queries
{
    /// <summary>
    /// Handler que obtiene el detalle completo de una Acción de Personal.
    /// Incluye lógica especial para acciones masivas: carga la lista de empleados afectados del lote.
    /// </summary>
    public class GetPersonalActionDetailQueryHandler
        : IRequestHandler<GetPersonalActionDetailQuery, PersonalActionDetailDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetPersonalActionDetailQueryHandler> _logger;

        public GetPersonalActionDetailQueryHandler(
            ApplicationDbContext context,
            ILogger<GetPersonalActionDetailQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PersonalActionDetailDto> Handle(
            GetPersonalActionDetailQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consultando detalle de acción de personal. Id: {ActionId}", request.ActionId);

            // ═══════════════════════════════════════════════════════════════
            // CONSULTA PRINCIPAL CON TODOS LOS JOINs
            // ═══════════════════════════════════════════════════════════════

            var action = await (
                from pa in _context.PersonalActions.AsNoTracking()

                    // JOIN con Employee
                join emp in _context.Employees.AsNoTracking()
                    on pa.EmployeeId equals emp.Id into empGroup
                from emp in empGroup.DefaultIfEmpty()

                    // JOIN con PayrollGroup
                join pg in _context.PayrollGroups.AsNoTracking()
                    on pa.PayrollGroupId equals pg.Id into pgGroup
                from pg in pgGroup.DefaultIfEmpty()

                    // JOIN con PayrollPeriod
                join pp in _context.PayrollPeriods.AsNoTracking()
                    on pa.TargetPayrollPeriodId equals pp.Id into ppGroup
                from pp in ppGroup.DefaultIfEmpty()

                    // JOIN con JobGrade (Antiguo)
                join oldJg in _context.JobGrades.AsNoTracking()
                    on pa.OldJobGradeId equals oldJg.Id into oldJgGroup
                from oldJg in oldJgGroup.DefaultIfEmpty()

                    // JOIN con JobGrade (Nuevo)
                join newJg in _context.JobGrades.AsNoTracking()
                    on pa.NewJobGradeId equals newJg.Id into newJgGroup
                from newJg in newJgGroup.DefaultIfEmpty()

                    // JOIN con ContractType (Antiguo)
                join oldCt in _context.ContractTypes.AsNoTracking()
                    on pa.OldContractTypeId equals oldCt.Id into oldCtGroup
                from oldCt in oldCtGroup.DefaultIfEmpty()

                    // JOIN con ContractType (Nuevo)
                join newCt in _context.ContractTypes.AsNoTracking()
                    on pa.NewContractTypeId equals newCt.Id into newCtGroup
                from newCt in newCtGroup.DefaultIfEmpty()

                    // JOIN con Shift (Antiguo)
                join oldSh in _context.Shifts.AsNoTracking()
                    on pa.OldShiftId equals oldSh.Id into oldShGroup
                from oldSh in oldShGroup.DefaultIfEmpty()

                    // JOIN con Shift (Nuevo)
                join newSh in _context.Shifts.AsNoTracking()
                    on pa.NewShiftId equals newSh.Id into newShGroup
                from newSh in newShGroup.DefaultIfEmpty()

                    // JOIN con CostCenter (Antiguo)
                join oldCc in _context.CostCenters.AsNoTracking()
                    on pa.OldCostCenterId equals oldCc.Id into oldCcGroup
                from oldCc in oldCcGroup.DefaultIfEmpty()

                    // JOIN con CostCenter (Nuevo)
                join newCc in _context.CostCenters.AsNoTracking()
                    on pa.NewCostCenterId equals newCc.Id into newCcGroup
                from newCc in newCcGroup.DefaultIfEmpty()

                    // ═══════════════════════════════════════════════════════════════
                    // NUEVOS JOINs AGREGADOS
                    // ═══════════════════════════════════════════════════════════════

                    // JOIN con HealthProvider (Antiguo)
                join oldHp in _context.HealthProviders.AsNoTracking()
                    on pa.OldHealthProviderId equals oldHp.Id into oldHpGroup
                from oldHp in oldHpGroup.DefaultIfEmpty()

                    // JOIN con HealthProvider (Nuevo)
                join newHp in _context.HealthProviders.AsNoTracking()
                    on pa.NewHealthProviderId equals newHp.Id into newHpGroup
                from newHp in newHpGroup.DefaultIfEmpty()

                    // JOIN con Bank (Antiguo)
                join oldBk in _context.Banks.AsNoTracking()
                    on pa.OldBankId equals oldBk.Id into oldBkGroup
                from oldBk in oldBkGroup.DefaultIfEmpty()

                    // JOIN con Bank (Nuevo)
                join newBk in _context.Banks.AsNoTracking()
                    on pa.NewBankId equals newBk.Id into newBkGroup
                from newBk in newBkGroup.DefaultIfEmpty()

                    // JOIN con User (Aprobador/Ejecutor)
                join user in _context.Users.AsNoTracking()
                    on pa.ApprovedBy equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()

                    // JOIN con User (Creador) - usando CreatedBy de BaseEntity
                join creator in _context.Users.AsNoTracking()
                    on pa.CreatedBy equals creator.Id into creatorGroup
                from creator in creatorGroup.DefaultIfEmpty()

                where pa.Id == request.ActionId
                select new PersonalActionDetailDto
                {
                    // Datos básicos
                    Id = pa.Id,
                    EffectiveDate = pa.EffectiveDate,
                    BatchReference = pa.BatchReference,

                    // Datos del empleado
                    EmployeeCode = emp != null ? emp.Code : null,
                    EmployeeName = emp != null ? (emp.FirstName + " " + emp.LastName) : null,
                    EmployeeIdentification = emp != null ? emp.Identification : null,
                    EmployeePosition = emp != null ? emp.JobGrade.Name : null,

                    // Contexto de nómina
                    PayrollGroupId = pa.PayrollGroupId,
                    PayrollGroupName = pg != null ? pg.Name : "Desconocido",
                    TargetPayrollPeriodId = pa.TargetPayrollPeriodId,
                    PayrollPeriodName = pp != null ? pp.PeriodNumber.ToString() : "Desconocido",
                    PayrollPeriodStatus = pp != null ? pp.Status.ToString() : "Desconocido",

                    // Tipo y causal
                    ActionType = pa.ActionType.ToString(),
                    ActionTypeDescription = GetActionTypeDescription(pa.ActionType),
                    CausalDescription = pa.CausalDescription,

                    // Estado
                    Status = pa.Status.ToString(),

                    // Justificación y documentos
                    Justification = pa.Justification,
                    DocumentAttachmentsJson = pa.DocumentAttachments, // Campo temporal para parsear después

                    // ═══════════════════════════════════════════════════════════════
                    // CAMPOS DE SUSPENSIÓN (NUEVOS)
                    // ═══════════════════════════════════════════════════════════════
                    SuspensionType = pa.SuspensionType.ToString(),
                    SuspensionStartDate = pa.SuspensionStartDate,
                    SuspensionEndDate = pa.SuspensionEndDate,

                    // ═══════════════════════════════════════════════════════════════
                    // CAMPOS DE TERMINACIÓN (NUEVOS)
                    // ═══════════════════════════════════════════════════════════════
                    TerminationTypeName = pa.TerminationTypeName,
                    IndemnityAmount = pa.IndemnityAmount,
                    IsJustified = pa.IsJustified,
                    RejectionReason = pa.RejectionReason,

                    // Auditoría
                    CreatedDate = pa.CreatedAt,
                    CreatedByName = creator != null ? (creator.UserName ?? "Sistema") : "Sistema",
                    ApprovedDate = pa.ExecutedDate,
                    ApprovedByName = user != null ? (user.UserName ?? "Sistema") : "Sistema",
                    ExecutedDate = pa.ExecutedDate,
                    ExecutedByName = user != null ? (user.UserName ?? "Sistema") : "Sistema",

                    // Valores fuertemente tipados - Salario
                    OldBaseSalary = pa.OldBaseSalary,
                    NewBaseSalary = pa.NewBaseSalary,

                    // Valores fuertemente tipados - Puesto
                    OldJobGradeId = pa.OldJobGradeId,
                    OldJobGradeName = oldJg != null ? oldJg.Name : null,
                    NewJobGradeId = pa.NewJobGradeId,
                    NewJobGradeName = newJg != null ? newJg.Name : null,

                    // Valores fuertemente tipados - Contrato
                    OldContractTypeId = pa.OldContractTypeId,
                    OldContractTypeName = oldCt != null ? oldCt.Name : null,
                    NewContractTypeId = pa.NewContractTypeId,
                    NewContractTypeName = newCt != null ? newCt.Name : null,

                    // Valores fuertemente tipados - Turno
                    OldShiftId = pa.OldShiftId,
                    OldShiftName = oldSh != null ? oldSh.Name : null,
                    NewShiftId = pa.NewShiftId,
                    NewShiftName = newSh != null ? newSh.Name : null,

                    // Valores fuertemente tipados - Centro de Costo
                    OldCostCenterId = pa.OldCostCenterId,
                    OldCostCenterName = oldCc != null ? oldCc.Name : null,
                    NewCostCenterId = pa.NewCostCenterId,
                    NewCostCenterName = newCc != null ? newCc.Name : null,

                    // ═══════════════════════════════════════════════════════════════
                    // VALORES FUERTEMENTE TIPADOS - PROVEEDOR DE SALUD (NUEVOS)
                    // ═══════════════════════════════════════════════════════════════
                    OldHealthProviderId = pa.OldHealthProviderId,
                    OldHealthProviderName = oldHp != null ? oldHp.Name : null,
                    NewHealthProviderId = pa.NewHealthProviderId,
                    NewHealthProviderName = newHp != null ? newHp.Name : null,

                    // ═══════════════════════════════════════════════════════════════
                    // VALORES FUERTEMENTE TIPADOS - BANCO Y CUENTA (NUEVOS)
                    // ═══════════════════════════════════════════════════════════════
                    OldBankId = pa.OldBankId,
                    OldBankName = oldBk != null ? oldBk.Name : null,
                    NewBankId = pa.NewBankId,
                    NewBankName = newBk != null ? newBk.Name : null,
                    OldBankAccountNumber = pa.OldBankAccountNumber,
                    NewBankAccountNumber = pa.NewBankAccountNumber,

                    // Control de procesamiento de nómina
                    IsAppliedInPayroll = pa.IsAppliedInPayroll,
                    AppliedInPayrollDate = pa.AppliedInPayrollDate,
                    AppliedInPayrollRecordId = pa.AppliedInPayrollRecordId
                }
            ).FirstOrDefaultAsync(cancellationToken);

            // ═══════════════════════════════════════════════════════════════
            // VALIDAR QUE LA ACCIÓN EXISTA
            // ═══════════════════════════════════════════════════════════════

            if (action == null)
            {
                _logger.LogWarning("Acción de personal no encontrada. Id: {ActionId}", request.ActionId);
                throw new KeyNotFoundException($"No se encontró la acción de personal con Id {request.ActionId}");
            }

            // ═══════════════════════════════════════════════════════════════
            // PARSEAR DOCUMENTOS ADJUNTOS (JSON -> List<DocumentAttachmentDto>)
            // ═══════════════════════════════════════════════════════════════

            // Parsear el JSON de documentos adjuntos
            action.DocumentAttachments = ParseDocumentAttachments(action.DocumentAttachmentsJson);

            // ═══════════════════════════════════════════════════════════════
            // LÓGICA ESPECIAL PARA ACCIONES MASIVAS
            // ═══════════════════════════════════════════════════════════════

            if (!string.IsNullOrEmpty(action.BatchReference))
            {
                _logger.LogInformation(
                    "Acción masiva detectada. BatchReference: {BatchRef}. Cargando lista de empleados afectados.",
                    action.BatchReference);

                // Consultar todas las acciones del mismo lote
                var batchActions = await (
                    from pa in _context.PersonalActions.AsNoTracking()
                    join emp in _context.Employees.AsNoTracking()
                        on pa.EmployeeId equals emp.Id
                    where pa.BatchReference == action.BatchReference
                    orderby emp.Code
                    select new EmployeeBriefDto
                    {
                        Id = emp.Id,
                        Code = emp.Code,
                        FullName = emp.FirstName + " " + emp.LastName,
                        Identification = emp.Identification,
                        Position = emp.JobGrade.Name
                    }
                ).ToListAsync(cancellationToken);

                action.AffectedEmployees = batchActions;
                action.AffectedCount = batchActions.Count;

                _logger.LogInformation(
                    "Cargados {Count} empleados del lote {BatchRef}",
                    batchActions.Count, action.BatchReference);
            }

            _logger.LogInformation("Detalle de acción cargado exitosamente. Id: {ActionId}", request.ActionId);

            return action;
        }

        // ═══════════════════════════════════════════════════════════════
        // MÉTODO AUXILIAR: Parsear documentos adjuntos desde JSON
        // ═══════════════════════════════════════════════════════════════

        private static List<DocumentAttachmentDto> ParseDocumentAttachments(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<DocumentAttachmentDto>();
            }

            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                // Limpiar espacios en blanco al inicio
                var trimmedJson = json.TrimStart();

                // ═══════════════════════════════════════════════════════════════
                // FORMATO 1: Array directo (correcto)
                // Ejemplo: [{"FileName":"doc.pdf","FileUrl":"/uploads/xxx.pdf",...}]
                // ═══════════════════════════════════════════════════════════════
                if (trimmedJson.StartsWith("["))
                {
                    return JsonSerializer.Deserialize<List<DocumentAttachmentDto>>(json, options)
                           ?? new List<DocumentAttachmentDto>();
                }

                // ═══════════════════════════════════════════════════════════════
                // FORMATO 2: Objeto con propiedad "documents" (incorrecto)
                // Ejemplo: {"documents":[...],"actionData":{...}}
                // ═══════════════════════════════════════════════════════════════
                if (trimmedJson.StartsWith("{"))
                {
                    using var doc = JsonDocument.Parse(json);

                    // Intentar extraer la propiedad "documents"
                    if (doc.RootElement.TryGetProperty("documents", out var documentsElement))
                    {
                        var documentsJson = documentsElement.GetRawText();
                        return JsonSerializer.Deserialize<List<DocumentAttachmentDto>>(documentsJson, options)
                               ?? new List<DocumentAttachmentDto>();
                    }

                    // Si no tiene "documents", intentar con "DocumentAttachments" (otra variante)
                    if (doc.RootElement.TryGetProperty("DocumentAttachments", out var attachmentsElement))
                    {
                        var attachmentsJson = attachmentsElement.GetRawText();
                        return JsonSerializer.Deserialize<List<DocumentAttachmentDto>>(attachmentsJson, options)
                               ?? new List<DocumentAttachmentDto>();
                    }
                }

                // Si no coincide con ningún formato conocido, retornar lista vacía
                return new List<DocumentAttachmentDto>();
            }
            catch (JsonException)
            {
                // Si falla el parseo, retornar lista vacía
                return new List<DocumentAttachmentDto>();
            }
            catch (Exception)
            {
                // Capturar cualquier otra excepción
                return new List<DocumentAttachmentDto>();
            }
        }

        // ═══════════════════════════════════════════════════════════════
        // MÉTODO AUXILIAR: Descripciones legibles de ActionType
        // ═══════════════════════════════════════════════════════════════

        private static string GetActionTypeDescription(ActionType actionType) => actionType switch
        {
            ActionType.SalaryChange => "💰 Cambio de Salario",
            ActionType.PositionChange => "🔄 Cambio de Puesto",
            ActionType.ContractChange => "📄 Cambio de Contrato",
            ActionType.ShiftChange => "⏰ Cambio de Turno",
            ActionType.CostCenterChange => "🏢 Cambio de Centro de Costo",
            ActionType.HealthProviderChange => "🏥 Cambio de Proveedor de Salud",
            ActionType.BankAccountChange => "🏦 Cambio de Cuenta Bancaria",
            ActionType.Suspension => "⏸️ Suspensión",
            ActionType.MassSuspension => "🏭 Suspensión Colectiva",
            ActionType.Reinstatement => "▶️ Reincorporación",
            ActionType.Termination => "🚪 Terminación",
            ActionType.MassTermination => "🏚️ Terminación Masiva",
            _ => actionType.ToString()
        };
    }
}