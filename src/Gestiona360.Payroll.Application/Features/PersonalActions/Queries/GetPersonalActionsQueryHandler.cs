using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Handler que ejecuta la consulta paginada de Acciones de Personal.
    /// Implementa LEFT JOINs con todas las entidades relacionadas para obtener nombres legibles.
    /// </summary>
    public class GetPersonalActionsQueryHandler
        : IRequestHandler<GetPersonalActionsQuery, PersonalActionPagedResultDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<GetPersonalActionsQueryHandler> _logger;

        public GetPersonalActionsQueryHandler(
            ApplicationDbContext context,
            ILogger<GetPersonalActionsQueryHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<PersonalActionPagedResultDto> Handle(
            GetPersonalActionsQuery request,
            CancellationToken cancellationToken)
        {
            var filter = request.Filter;

            _logger.LogInformation(
                "Consultando acciones de personal. Período: {PeriodId}, Página: {Page}, Tamaño: {Size}",
                filter.PayrollPeriodId, filter.PageNumber, filter.PageSize);

            // ═══════════════════════════════════════════════════════════════
            // QUERY BASE CON LEFT JOINS
            // ═══════════════════════════════════════════════════════════════

            var query = (
                from pa in _context.PersonalActions.AsNoTracking()

                    // JOIN con Employee
                join emp in _context.Employees.AsNoTracking()
                    on pa.EmployeeId equals emp.Id into empGroup
                from emp in empGroup.DefaultIfEmpty()

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

                    // JOIN con User (Aprobador/Ejecutor)
                join user in _context.Users.AsNoTracking()
                    on pa.ApprovedBy equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()

                    // ═══════════════════════════════════════════════════════════════
                    // FILTRO OBLIGATORIO POR PERÍODO DE NÓMINA
                    // ═══════════════════════════════════════════════════════════════
                where pa.TargetPayrollPeriodId == filter.PayrollPeriodId

                // ═══════════════════════════════════════════════════════════════
                // FILTROS OPCIONALES
                // ═══════════════════════════════════════════════════════════════

                // Filtro por grupo de nómina (si se proporciona)
                && (filter.PayrollGroupId == null || pa.PayrollGroupId == filter.PayrollGroupId)

                // Filtro por tipo de acción
                && (string.IsNullOrEmpty(filter.ActionType) || pa.ActionType.ToString() == filter.ActionType)

                // Filtro por estado
                && (string.IsNullOrEmpty(filter.Status) || pa.Status.ToString() == filter.Status)

                // Filtro por referencia de lote
                && (string.IsNullOrEmpty(filter.BatchReference) || pa.BatchReference == filter.BatchReference)

                // Filtro por empleado específico
                && (filter.EmployeeId == null || pa.EmployeeId == filter.EmployeeId)

                // Filtro de búsqueda libre (nombre, código, cédula, causal, referencia)
                && (string.IsNullOrEmpty(filter.SearchTerm) ||
                    (emp != null && (
                        (emp.FirstName + " " + emp.LastName).Contains(filter.SearchTerm) ||
                        emp.Code.Contains(filter.SearchTerm) ||
                        emp.Identification.Contains(filter.SearchTerm)
                    )) ||
                    (pa.CausalDescription != null && pa.CausalDescription.Contains(filter.SearchTerm)) ||
                    (pa.BatchReference != null && pa.BatchReference.Contains(filter.SearchTerm)))

                // ═══════════════════════════════════════════════════════════════
                // PROYECCIÓN AL DTO
                // ═══════════════════════════════════════════════════════════════
                select new PersonalActionListDto
                {
                    // Identificadores
                    Id = pa.Id,
                    EffectiveDate = pa.EffectiveDate,
                    BatchReference = pa.BatchReference,

                    // Datos del empleado
                    EmployeeCode = emp != null ? emp.Code : null,
                    EmployeeName = emp != null ? (emp.FirstName + " " + emp.LastName) : null,
                    EmployeeIdentification = emp != null ? emp.Identification : null,

                    // Tipo y causal
                    ActionType = pa.ActionType.ToString(),
                    ActionTypeDescription = GetActionTypeDescription(pa.ActionType),
                    CausalDescription = pa.CausalDescription,

                    // Estado y auditoría
                    Status = pa.Status.ToString(),
                    ApprovedByName = user != null ? (user.UserName ?? user.UserName ?? "Sistema") : "Sistema",
                    ExecutedDate = pa.ExecutedDate,

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

                    // Control de procesamiento de nómina
                    IsAppliedInPayroll = pa.IsAppliedInPayroll,
                    AppliedInPayrollDate = pa.AppliedInPayrollDate
                }
            );

            // ═══════════════════════════════════════════════════════════════
            // ORDENAMIENTO
            // ═══════════════════════════════════════════════════════════════

            query = filter.SortBy?.ToLower() switch
            {
                "effectivedate" => filter.SortDescending
                    ? query.OrderByDescending(x => x.EffectiveDate)
                    : query.OrderBy(x => x.EffectiveDate),
                "employeename" => filter.SortDescending
                    ? query.OrderByDescending(x => x.EmployeeName)
                    : query.OrderBy(x => x.EmployeeName),
                "actiontype" => filter.SortDescending
                    ? query.OrderByDescending(x => x.ActionType)
                    : query.OrderBy(x => x.ActionType),
                "status" => filter.SortDescending
                    ? query.OrderByDescending(x => x.Status)
                    : query.OrderBy(x => x.Status),
                _ => query.OrderByDescending(x => x.EffectiveDate) // Default: fecha descendente
            };

            // ═══════════════════════════════════════════════════════════════
            // CONTAR TOTAL (antes de paginar)
            // ═══════════════════════════════════════════════════════════════

            var totalCount = await query.CountAsync(cancellationToken);

            // ═══════════════════════════════════════════════════════════════
            // PAGINACIÓN (Skip/Take)
            // ═══════════════════════════════════════════════════════════════

            var items = await query
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync(cancellationToken);

            // ═══════════════════════════════════════════════════════════════
            // CALCULAR AFFECTEDCOUNT PARA ACCIONES MASIVAS
            // ═══════════════════════════════════════════════════════════════

            // Obtener BatchReferences únicas de las acciones masivas en la página actual
            var batchReferences = items
                .Where(x => !string.IsNullOrEmpty(x.BatchReference))
                .Select(x => x.BatchReference)
                .Distinct()
                .ToList();

            if (batchReferences.Any())
            {
                // Contar empleados por cada BatchReference
                var batchCounts = await _context.PersonalActions
                    .Where(pa => batchReferences.Contains(pa.BatchReference))
                    .GroupBy(pa => pa.BatchReference)
                    .Select(g => new { BatchRef = g.Key, Count = g.Count() })
                    .ToDictionaryAsync(x => x.BatchRef!, x => x.Count, cancellationToken);

                // Asignar AffectedCount a cada acción masiva
                foreach (var item in items.Where(x => !string.IsNullOrEmpty(x.BatchReference)))
                {
                    if (batchCounts.TryGetValue(item.BatchReference!, out var count))
                    {
                        item.AffectedCount = count;
                    }
                }
            }

            _logger.LogInformation(
                "Consulta completada. Total: {Total}, Página: {Page}, Registros: {Count}",
                totalCount, filter.PageNumber, items.Count);

            // ═══════════════════════════════════════════════════════════════
            // RETORNAR RESULTADO PAGINADO
            // ═══════════════════════════════════════════════════════════════

            return new PersonalActionPagedResultDto
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
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
            ActionType.Suspension => "⏸️ Suspensión",
            ActionType.MassSuspension => "🏭 Suspensión Colectiva",
            ActionType.Reinstatement => "▶️ Reincorporación",
            ActionType.Termination => "🚪 Terminación",
            ActionType.MassTermination => "🏚️ Terminación Masiva",
            _ => actionType.ToString()
        };
    }
}
