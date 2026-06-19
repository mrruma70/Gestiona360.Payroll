using FluentValidation;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Interfaces;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class CreatePersonalActionValidator : AbstractValidator<CreatePersonalActionRequest>
    {
        private readonly IPersonalActionValidationService _validationService;

        public CreatePersonalActionValidator(IPersonalActionValidationService validationService)
        {
            _validationService = validationService;

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

                RuleFor(x => x)
                    .MustAsync(async (request, ct) =>
                    {
                        if (!request.NewBaseSalary.HasValue)
                            return false;

                        return await _validationService.IsSalaryAboveMinimumAsync(
                            request.EmployeeId,
                            request.NewBaseSalary.Value,
                            ct);
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

                RuleFor(x => x)
                    .MustAsync(async (request, ct) =>
                    {
                        return await _validationService.IsNewJobGradeDifferentAsync(
                            request.EmployeeId,
                            request.NewJobGradeId,
                            ct);
                    })
                    .WithMessage("El nuevo puesto debe ser diferente al actual.");

                RuleFor(x => x)
                    .MustAsync(async (request, ct) =>
                    {
                        return await _validationService.IsNewJobGradeSalaryValidAsync(
                            request.EmployeeId,
                            request.NewJobGradeId,
                            ct);
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

                RuleFor(x => x)
                    .MustAsync(async (request, ct) =>
                    {
                        return await _validationService.IsNewContractTypeDifferentAsync(
                            request.EmployeeId,
                            request.NewContractTypeId,
                            ct);
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

                RuleFor(x => x)
                    .MustAsync(async (request, ct) =>
                    {
                        return await _validationService.IsNewShiftDifferentAsync(
                            request.EmployeeId,
                            request.NewShiftId,
                            ct);
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

                RuleFor(x => x)
                    .MustAsync(async (request, ct) =>
                    {
                        return await _validationService.IsNewCostCenterDifferentAsync(
                            request.EmployeeId,
                            request.NewCostCenterId,
                            ct);
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

                RuleFor(x => x)
                    .MustAsync(async (request, ct) =>
                    {
                        return await _validationService.IsNewHealthProviderDifferentAsync(
                            request.EmployeeId,
                            request.NewHealthProviderId,
                            ct);
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

                RuleFor(x => x)
                    .MustAsync(async (request, ct) =>
                    {
                        return await _validationService.IsNewBankAccountDifferentAsync(
                            request.EmployeeId,
                            request.NewBankId,
                            request.NewBankAccountNumber,
                            ct);
                    })
                    .WithMessage("El nuevo banco o número de cuenta debe ser diferente al actual.");
            });

            // ==========================================================
            // SUSPENSION - Suspensión
            // ==========================================================
            When(x => x.ActionType == "Suspension", () =>
            {
                RuleFor(x => x.EndDate)
                    .GreaterThan(x => x.EffectiveDate)
                    .When(x => x.EndDate.HasValue)
                    .WithMessage("La fecha de fin debe ser posterior a la fecha de inicio.");

                RuleFor(x => x.EmployeeId)
                    .MustAsync(async (employeeId, ct) =>
                    {
                        return await _validationService.HasNoActiveSuspensionAsync(employeeId, ct);
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
                        return await _validationService.IsEmployeeSuspendedAsync(employeeId, ct);
                    })
                    .WithMessage("Solo se puede reincorporar a un empleado que esté en estado Suspendido.");
            });

            // ==========================================================
            // TERMINATION - Terminación
            // ==========================================================
            When(x => x.ActionType == "Termination", () =>
            {
                RuleFor(x => x.IsJustified)
                    .NotNull()
                    .WithMessage("Debe indicar si la terminación es justificada.");

                RuleFor(x => x.EmployeeId)
                    .MustAsync(async (employeeId, ct) =>
                    {
                        return await _validationService.CanBeTerminatedAsync(employeeId, ct);
                    })
                    .WithMessage("La terminación solo puede aplicarse a empleados Activos o Suspendidos (no terminados).");
            });
        }
    }
}