using System.Text.Json;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class CreateMassPersonalActionCommandHandler : IRequestHandler<CreateMassPersonalActionCommand, MassActionPreviewDto>
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateMassPersonalActionCommandHandler> _logger;

        public CreateMassPersonalActionCommandHandler(
            ApplicationDbContext context,
            IMediator mediator,
            ILogger<CreateMassPersonalActionCommandHandler> logger)
        {
            _context = context;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task<MassActionPreviewDto> Handle(CreateMassPersonalActionCommand request, CancellationToken cancellationToken)
        {
            var dto = request.Data;

            // 1. Obtener Vista Previa y Validar
            var previewQuery = new GetMassActionPreviewQuery(dto);
            var previewResult = await _mediator.Send(previewQuery, cancellationToken);

            if (previewResult.ValidationErrors.Any() || previewResult.EmployeesPreview.Any(e => !e.IsValid))
            {
                throw new InvalidOperationException("La acción masiva tiene errores de validación. Revise la vista previa.");
            }

            // 2. Iniciar Transacción
            using IDbContextTransaction transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var actionsToCreate = new List<PersonalAction>();
                var employees = await _context.Employees
                    .Where(e => dto.TargetEmployeeIds.Contains(e.Id))
                    .ToListAsync(cancellationToken);

                var targetPeriod = await _context.PayrollPeriods
                    .FirstOrDefaultAsync(p => p.PayrollGroupId == employees.First().PayrollGroupId &&
                                              dto.EffectiveDate >= p.StartDate &&
                                              dto.EffectiveDate <= p.EndDate, cancellationToken);

                foreach (var emp in employees)
                {
                    // Determinar NewValues según regla
                    decimal? newBaseSalary = null;
                    int? newStatus = null;

                    if (dto.ActionType == "SalaryChange")
                    {
                        newBaseSalary = dto.RuleType == "Fixed"
                            ? dto.RuleValue
                            : emp.BaseSalary * (1 + (dto.RuleValue ?? 0) / 100);
                    }
                    else if (dto.ActionType == "MassSuspension")
                    {
                        newStatus = (int)EmploymentStatus.Suspended;
                    }

                    var action = new PersonalAction
                    {
                        Id = Guid.NewGuid(),
                        EmployeeId = emp.Id,
                        PayrollGroupId = emp.PayrollGroupId,
                        TargetPayrollPeriodId = targetPeriod.Id,
                        ActionType = Enum.Parse<ActionType>(dto.ActionType), // Asumiendo mapeo correcto o usando string directly if needed
                        EffectiveDate = dto.EffectiveDate,
                        Status = ActionStatus.Executed, // Ejecución inmediata por defecto en masivo
                        CausalDescription = dto.CausalDescription,
                        Justification = dto.Justification,
                        BatchReference = dto.BatchReference,
                        IsAppliedInPayroll = false,

                        // Old Values
                        OldBaseSalary = emp.BaseSalary,
                        OldEmploymentStatus = (int)emp.EmploymentStatus,

                        // New Values
                        NewBaseSalary = newBaseSalary,
                        NewEmploymentStatus = newStatus,

                        DocumentAttachments = JsonSerializer.Serialize(dto.MasterDocuments),
                        CreatedAt = DateTime.UtcNow
                    };

                    actionsToCreate.Add(action);
                }

                // 3. Guardar Registros en BD
                await _context.PersonalActions.AddRangeAsync(actionsToCreate, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                // 4. Ejecutar Acciones Individualmente (Actualizar Tablas Maestras)
                // Nota: Para optimización extrema, esto podría hacerse en bulk, pero usamos el handler existente para consistencia.
                foreach (var action in actionsToCreate)
                {
                    var executeCommand = new ExecutePersonalActionCommand(action.Id);
                    await _mediator.Send(executeCommand, cancellationToken);
                }

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Acción masiva ejecutada exitosamente. Lote: {BatchRef}, Empleados: {Count}", dto.BatchReference, actionsToCreate.Count);

                // Retornar resumen actualizado
                previewResult.TotalEmployees = actionsToCreate.Count;
                return previewResult;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                _logger.LogError(ex, "Error al ejecutar acción masiva. Rollback realizado.");
                throw new InvalidOperationException("No se pudo completar la acción masiva. Se ha revertido la operación.", ex);
            }
        }
    }
}
