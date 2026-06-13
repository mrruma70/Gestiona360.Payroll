using FluentValidation;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class CreatePersonalActionValidator : AbstractValidator<CreatePersonalActionRequest>
    {
        private readonly ApplicationDbContext _context;

        public CreatePersonalActionValidator(ApplicationDbContext context)
        {
            _context = context;

            // ==========================================================
            // REGLAS GENERALES (aplican a todas las acciones)
            // ==========================================================
            RuleFor(x => x.EmployeeId)
                .NotEmpty()
                .WithMessage("El empleado es obligatorio.");

            RuleFor(x => x.ActionType)
                .NotEmpty()
                .WithMessage("El tipo de acción es obligatorio.");

            RuleFor(x => x.EffectiveDate)
                .NotEmpty()
                .WithMessage("La fecha de efecto es obligatoria.");

            RuleFor(x => x.Justification)
                .NotEmpty()
                .MinimumLength(20)
                .WithMessage("La justificación debe tener al menos 20 caracteres.");

            // ==========================================================
            // SALARY CHANGE - Cambio de Salario
            // ==========================================================
            When(x => x.ActionType == "SalaryChange", () =>
            {
                RuleFor(x => x.NewBaseSalary)
                    .NotNull()
                    .GreaterThan(0)
                    .WithMessage("El nuevo salario debe ser mayor a 0.");

                RuleFor(x => x.NewBaseSalary)
                    .MustAsync(async (request, salary, ct) =>
                    {
                        if (!salary.HasValue)
                            return false;

                        var employee = await _context.Employees
                            .Include(e => e.JobGrade)
                                .ThenInclude(jg => jg.JobPosition)
                            .FirstOrDefaultAsync(
                                e => e.Id == request.EmployeeId,
                                ct);

                        if (employee == null)
                            return false;

                        if (employee.JobGrade == null ||
                            employee.JobGrade.JobPosition == null)
                            return false;

                        var minimumWageId =
                            employee.JobGrade.JobPosition.MinimumWageId;

                        // Si el puesto no tiene salario mínimo configurado
                        if (!minimumWageId.HasValue)
                            return true;

                        var minimumWage = await _context.MinimumWages
                            .FirstOrDefaultAsync(
                                mw => mw.Id == minimumWageId.Value &&
                                      mw.IsActive,
                                ct);

                        // Si no existe configuración, no bloquear
                        if (minimumWage == null)
                            return true;

                        return salary.Value >= minimumWage.MonthlyAmountNIO;
                    })
                    .WithMessage("El nuevo salario no puede ser inferior al salario mínimo legal configurado para el puesto.");
            });

            // ==========================================================
            // POSITION CHANGE - Cambio de Puesto
            // ==========================================================
            When(x => x.ActionType == "PositionChange", () =>
            {
                RuleFor(x => x.NewJobGradeId)
                    .NotNull()
                    .WithMessage("Debe seleccionar un nuevo puesto.");

                // Validación de que el nuevo puesto sea diferente al actual
                RuleFor(x => x.NewJobGradeId)
                    .MustAsync(async (request, newJobGradeId, ct) =>
                    {
                        if (!newJobGradeId.HasValue) return false;
                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, ct);
                        if (employee == null) return false;
                        if (!employee.JobGradeId.HasValue) return true;
                        return newJobGradeId.Value != employee.JobGradeId.Value;
                    })
                    .WithMessage("El nuevo puesto debe ser diferente al actual.");

                // Validación de salario mínimo del nuevo puesto
                RuleFor(x => new { x.NewJobGradeId, x.EmployeeId })
                    .MustAsync(async (obj, ct) =>
                    {
                        if (!obj.NewJobGradeId.HasValue) return true;

                        var employee = await _context.Employees
                            .Include(e => e.JobGrade)
                                .ThenInclude(jg => jg.JobPosition)
                                    .ThenInclude(jp => jp.MinimumWage)
                            .FirstOrDefaultAsync(e => e.Id == obj.EmployeeId, ct);
                        if (employee == null) return true;

                        var newJobGrade = await _context.JobGrades
                            .Include(jg => jg.JobPosition)
                                .ThenInclude(jp => jp.MinimumWage)
                            .FirstOrDefaultAsync(jg => jg.Id == obj.NewJobGradeId, ct);
                        if (newJobGrade == null) return true;

                        var minimumWage = newJobGrade.JobPosition?.MinimumWage?.MonthlyAmountNIO;
                        if (!minimumWage.HasValue) return true; // No hay mínimo definido, se permite

                        // El salario actual del empleado no puede ser inferior al mínimo del nuevo puesto
                        return employee.BaseSalary >= minimumWage.Value;
                    })
                    .WithMessage("El salario actual del empleado es inferior al salario mínimo establecido para el nuevo puesto. Debe realizar un ajuste salarial primero.");
            });

            // ==========================================================
            // CONTRACT CHANGE - Cambio de Tipo de Contrato
            // ==========================================================
            When(x => x.ActionType == "ContractChange", () =>
            {
                RuleFor(x => x.NewContractTypeId)
                    .NotNull()
                    .WithMessage("Debe seleccionar un nuevo tipo de contrato.");

                RuleFor(x => x.NewContractTypeId)
                    .MustAsync(async (request, newContractTypeId, ct) =>
                    {
                        if (!newContractTypeId.HasValue) return false;

                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, ct);
                        if (employee == null) return false;

                        if (!employee.ContractTypeId.HasValue) return true;

                        return newContractTypeId.Value != employee.ContractTypeId.Value;
                    })
                    .WithMessage("El nuevo tipo de contrato debe ser diferente al actual.");
            });

            // ==========================================================
            // SHIFT CHANGE - Cambio de Turno
            // ==========================================================
            When(x => x.ActionType == "ShiftChange", () =>
            {
                RuleFor(x => x.NewShiftId)
                    .NotNull()
                    .WithMessage("Debe seleccionar un nuevo turno.");

                RuleFor(x => x.NewShiftId)
                    .MustAsync(async (request, newShiftId, ct) =>
                    {
                        if (!newShiftId.HasValue) return false;

                        var activeShift = await _context.EmployeeShiftAssignments
                            .FirstOrDefaultAsync(sa => sa.EmployeeId == request.EmployeeId && sa.EndDate == null, ct);

                        // Si el empleado NO tiene turno actual, cualquier turno es válido
                        if (activeShift == null) return true;

                        // Si tiene turno actual, debe ser diferente
                        return newShiftId.Value != activeShift.ShiftId;
                    })
                    .WithMessage("El nuevo turno debe ser diferente al turno actual del empleado.");
            });

            // ==========================================================
            // COST CENTER CHANGE - Cambio de Centro de Costo
            // ==========================================================
            When(x => x.ActionType == "CostCenterChange", () =>
            {
                RuleFor(x => x.NewCostCenterId)
                    .NotNull()
                    .WithMessage("Debe seleccionar un nuevo centro de costo.");

                RuleFor(x => x.NewCostCenterId)
                    .MustAsync(async (request, newCostCenterId, ct) =>
                    {
                        if (!newCostCenterId.HasValue) return false;

                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, ct);
                        if (employee == null) return false;

                        if (!employee.CostCenterId.HasValue) return true;

                        return newCostCenterId.Value != employee.CostCenterId.Value;
                    })
                    .WithMessage("El nuevo centro de costo debe ser diferente al actual.");
            });

            // ==========================================================
            // HEALTH PROVIDER CHANGE - Cambio de Proveedor de Salud
            // ==========================================================
            When(x => x.ActionType == "HealthProviderChange", () =>
            {
                RuleFor(x => x.NewHealthProviderId)
                    .NotNull()
                    .WithMessage("Debe seleccionar un nuevo proveedor de salud.");

                RuleFor(x => x.NewHealthProviderId)
                    .MustAsync(async (request, newHealthProviderId, ct) =>
                    {
                        if (!newHealthProviderId.HasValue) return false;

                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, ct);
                        if (employee == null) return false;

                        if (!employee.HealthProviderId.HasValue) return true;

                        return newHealthProviderId.Value != employee.HealthProviderId.Value;
                    })
                    .WithMessage("El nuevo proveedor de salud debe ser diferente al actual.");
            });

            // ==========================================================
            // BANK ACCOUNT CHANGE - Cambio de Cuenta Bancaria
            // ==========================================================
            When(x => x.ActionType == "BankAccountChange", () =>
            {
                RuleFor(x => x.NewBankId)
                    .NotNull()
                    .WithMessage("Debe seleccionar un nuevo banco.");

                RuleFor(x => x.NewBankAccountNumber)
                    .NotEmpty()
                    .WithMessage("Debe ingresar el número de cuenta bancaria.");

                RuleFor(x => x.NewBankAccountNumber)
                    .MustAsync(async (request, newAccountNumber, ct) =>
                    {
                        if (string.IsNullOrWhiteSpace(newAccountNumber)) return false;

                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, ct);
                        if (employee == null) return false;

                        // Si no tiene banco o cuenta anterior, cualquier combinación es válida
                        if (!employee.BankId.HasValue || string.IsNullOrWhiteSpace(employee.BankAccountNumber))
                            return true;

                        // Debe cambiar al menos uno de los dos: banco o número de cuenta
                        return request.NewBankId != employee.BankId ||
                               newAccountNumber.Trim() != employee.BankAccountNumber.Trim();
                    })
                    .WithMessage("El nuevo banco o número de cuenta debe ser diferente al actual.");
            });

            // ==========================================================
            // SUSPENSION - Suspensión (reglas consolidadas)
            // ==========================================================
            When(x => x.ActionType == "Suspension", () =>
            {
                // Validación de fechas
                RuleFor(x => x.EndDate)
                    .GreaterThan(x => x.EffectiveDate)
                    .When(x => x.EndDate.HasValue)
                    .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

                // Validación de suspensión activa
                RuleFor(x => x.EmployeeId)
                    .MustAsync(async (employeeId, ct) =>
                    {
                        var activeSuspension = await _context.EmployeeSuspensions
                            .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.EndDate == null, ct);
                        return activeSuspension == null;
                    })
                    .WithMessage("El empleado ya tiene una suspensión activa. No se puede crear otra.");
            });

            // ==========================================================
            // REINSTATEMENT - Reincorporación
            // ==========================================================
            When(x => x.ActionType == "Reinstatement", () =>
            {
                RuleFor(x => x.EmployeeId)
                    .MustAsync(async (employeeId, ct) =>
                    {
                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Id == employeeId, ct);
                        return employee != null && employee.EmploymentStatus == EmploymentStatus.Suspended;
                    })
                    .WithMessage("Solo se puede reincorporar a un empleado que esté en estado Suspendido.");
            });

            // ==========================================================
            // TERMINATION - Terminación (reglas consolidadas)
            // ==========================================================
            When(x => x.ActionType == "Termination", () =>
            {
                // Validación de justificación
                RuleFor(x => x.IsJustified)
                    .NotNull()
                    .WithMessage("Debe indicar si la terminación es justificada.");

                // Validación de estado del empleado
                RuleFor(x => x.EmployeeId)
                    .MustAsync(async (employeeId, ct) =>
                    {
                        var employee = await _context.Employees
                            .FirstOrDefaultAsync(e => e.Id == employeeId, ct);
                        return employee != null &&
                               (employee.EmploymentStatus == EmploymentStatus.Active ||
                                employee.EmploymentStatus == EmploymentStatus.Suspended);
                    })
                    .WithMessage("La terminación solo puede aplicarse a empleados Activos o Suspendidos (no terminados).");
            });
        }
    }
}