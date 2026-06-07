using FluentValidation;
using Gestiona360.Payroll.Application.Commands.PayrollFrequency;
using Gestiona360.Payroll.Application.Contracts.Requests;

namespace Gestiona360.Payroll.Application.Validators
{
    public class CreatePayrollFrequencyValidator : AbstractValidator<CreatePayrollFrequencyCommand>
    {
        public CreatePayrollFrequencyValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre es obligatorio.")
                .MaximumLength(20).WithMessage("Máximo 20 caracteres.");

            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("El código es obligatorio.")
                .MaximumLength(20).WithMessage("Máximo 20 caracteres.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("La descripción es obligatoria.")
                .MaximumLength(200).WithMessage("Máximo 200 caracteres.");

            RuleFor(x => x.DaysPerPeriod)
                .GreaterThan(0).WithMessage("Debe ser mayor a 0.")
                .LessThanOrEqualTo(365).WithMessage("No puede superar 365.");

            RuleFor(x => x.PeriodsPerYear)
                .GreaterThan(0).WithMessage("Debe ser mayor a 0.")
                .LessThanOrEqualTo(12).WithMessage("No puede superar 12.");

            RuleFor(x => x)
                .Must(req => req.DaysPerPeriod * req.PeriodsPerYear <= 365)
                .WithMessage("La combinación de días y períodos excede los días de un año.");
        }
    }
}


