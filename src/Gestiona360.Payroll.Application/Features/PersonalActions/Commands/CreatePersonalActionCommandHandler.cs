using System.Text.Json;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class CreatePersonalActionCommandHandler : IRequestHandler<CreatePersonalActionCommand, Guid>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ILogger<CreatePersonalActionCommandHandler> _logger;

        public CreatePersonalActionCommandHandler(
            ApplicationDbContext context,
            IMediator mediator,
            ILogger<CreatePersonalActionCommandHandler> logger)
        {
            _context = context;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<Guid> Handle(CreatePersonalActionCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Data;

            // 1. Obtener empleado y validar existencia
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == dto.EmployeeId, cancellationToken)
                ?? throw new KeyNotFoundException($"Empleado con ID {dto.EmployeeId} no encontrado.");

            // Validar que el empleado tenga un grupo de nómina asignado
            if (employee.PayrollGroupId == Guid.Empty)
                throw new InvalidOperationException("El empleado no tiene un Grupo de Nómina asignado.");

            // 2. Determinar automáticamente el TargetPayrollPeriodId
            var targetPeriod = await _context.PayrollPeriods
                .FirstOrDefaultAsync(p => p.PayrollGroupId == employee.PayrollGroupId &&
                                          dto.EffectiveDate >= p.StartDate &&
                                          dto.EffectiveDate <= p.EndDate, cancellationToken);

            if (targetPeriod == null)
                throw new InvalidOperationException($"No existe un período de nómina abierto o configurado para la fecha {dto.EffectiveDate:dd/MM/yyyy} en el grupo del empleado.");

            // 3. Validar que el período esté "Open"
            if (targetPeriod.Status != "Open")
                throw new InvalidOperationException($"No se pueden crear acciones porque el período '{targetPeriod.PeriodNumber.ToString()}' está en estado '{targetPeriod.Status}'.");

            // 4. Validaciones de Negocio
            if (string.IsNullOrWhiteSpace(dto.Justification) || dto.Justification.Length < 20)
                throw new ArgumentException("La justificación debe tener al menos 20 caracteres.");

            // Validación de Salario Mínimo (si aplica)
            if (dto.ActionType == "SalaryChange" && dto.NewBaseSalary.HasValue)
            {
                if (dto.NewBaseSalary.Value <= 0)
                    throw new ArgumentException("El nuevo salario debe ser mayor a cero.");

                // TODO: Inyectar servicio de Salarios Mínimos para validar contra el sector del empleado
                // var minWage = await _minimumWageService.GetMinimumWage(employee.SectorId, dto.EffectiveDate);
                // if (dto.NewBaseSalary.Value < minWage) 
                //     throw new ValidationException($"El salario propuesto es inferior al mínimo legal ({minWage}).");
            }

            // Obtener el turno activo actual del empleado (si existe)
            var activeShift = await _context.EmployeeShiftAssignments
                .FirstOrDefaultAsync(sa => sa.EmployeeId == employee.Id && sa.EndDate == null, cancellationToken);

            // Capturar valores actuales para Bank y HealthProvider
            var oldBankId = employee.BankId;
            var oldBankAccountNumber = employee.BankAccountNumber;
            var oldHealthProviderId = employee.HealthProviderId;

            // 5. Capturar OldValues desde la tabla Employees
            var action = new PersonalAction
            {
                Id = Guid.NewGuid(),
                EmployeeId = dto.EmployeeId,
                PayrollGroupId = employee.PayrollGroupId,
                TargetPayrollPeriodId = targetPeriod.Id,
                ActionType = Enum.Parse<ActionType>(dto.ActionType),
                EffectiveDate = dto.EffectiveDate,
                Status = dto.ExecuteImmediately ? ActionStatus.Executed : ActionStatus.Pending,
                CausalDescription = dto.CausalDescription,
                Justification = dto.Justification,
                OldShiftId = activeShift?.ShiftId,

                // Control de nómina
                IsAppliedInPayroll = false,

                // Captura de OldValues (Estado actual del maestro)
                OldBaseSalary = employee.BaseSalary,
                OldJobGradeId = employee.JobGradeId,
                OldContractTypeId = employee.ContractTypeId,
                OldCostCenterId = employee.CostCenterId,
                OldEmploymentStatus = (int)employee.EmploymentStatus,

                //  Old values para banco y proveedor de salud
                OldBankId = oldBankId,
                OldBankAccountNumber = oldBankAccountNumber,
                OldHealthProviderId = oldHealthProviderId,

                // Nuevos valores (del DTO)
                NewBaseSalary = dto.NewBaseSalary,
                NewJobGradeId = dto.NewJobGradeId,
                NewContractTypeId = dto.NewContractTypeId,
                NewCostCenterId = dto.NewCostCenterId,
                NewShiftId = dto.NewShiftId,

                //New values para banco y proveedor de salud
                NewBankId = dto.NewBankId,
                NewBankAccountNumber = dto.NewBankAccountNumber,
                NewHealthProviderId = dto.NewHealthProviderId,

                // Documentos (serializados a JSON)
                DocumentAttachments = dto.Documents != null && dto.Documents.Any()
                    ? JsonSerializer.Serialize(dto.Documents)
                    : "[]",

                // Auditoría básica (CreatedBy se maneja vía Interceptor o DbContext override si está configurado)
                CreatedAt = DateTime.UtcNow
            };

            // Si es suspensión, capturar fechas específicas si vienen en el DTO
            if (dto.ActionType == "Suspension" || dto.ActionType == "MassSuspension")
            {
                action.NewEmploymentStatus = (int)EmploymentStatus.Suspended;
                // Nota: Las fechas de inicio/fin de suspensión se guardan en EmployeeSuspensions al ejecutar,
                // pero podríamos guardarlas aquí si el diseño lo requiere. Por ahora, seguimos el flujo de ejecución.
            }

            if (dto.ActionType == "BankAccountChange")
            {
                var extraData = new Dictionary<string, object>
                {
                    ["NewBankId"] = dto.NewBankId ?? 0,
                    ["NewBankAccountNumber"] = dto.NewBankAccountNumber ?? ""
                };
                var existingDocs = dto.Documents ?? new List<DocumentAttachmentDto>();
                var combined = new { documents = existingDocs, actionData = extraData };
                action.DocumentAttachments = JsonSerializer.Serialize(combined);
            }
            else
            {
                action.DocumentAttachments = dto.Documents != null && dto.Documents.Any()
                    ? JsonSerializer.Serialize(dto.Documents)
                    : "[]";
            }

            _context.PersonalActions.Add(action);

            // 6. Lógica de Ejecución Inmediata vs Pendiente
            if (dto.ExecuteImmediately)
            {
                // Usamos una transacción para asegurar que la creación y la actualización maestra sean atómicas
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    // Guardamos primero para tener el ID generado si se necesita en logs o referencias
                    await _context.SaveChangesAsync(cancellationToken);

                    // Ejecutamos la acción internamente
                    // Nota: Reutilizamos la lógica del ExecutePersonalActionCommand para mantener consistencia
                    var executeCommand = new ExecutePersonalActionCommand(action.Id);
                    await _mediator.Send(executeCommand, cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                    _logger.LogInformation("Acción de personal creada y ejecutada inmediatamente. Id: {ActionId}", action.Id);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error al ejecutar inmediatamente la acción de personal. Id: {ActionId}", action.Id);
                    throw new InvalidOperationException("No se pudo ejecutar la acción inmediatamente. Verifique las reglas de negocio.", ex);
                }
            }
            else
            {
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Acción de personal creada como Pendiente. Id: {ActionId}", action.Id);
            }

            return action.Id;
        }
    }
}

