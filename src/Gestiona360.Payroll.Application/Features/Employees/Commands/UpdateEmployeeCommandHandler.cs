using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands
{
    public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
    {
        private readonly ApplicationDbContext _context;

        public UpdateEmployeeCommandHandler(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Unit> Handle(UpdateEmployeeCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            // Buscar empleado existente
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == request.Id, cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException($"Empleado con ID {request.Id} no encontrado.");

            // Validar que el JobGrade existe y está activo
            var jobGrade = await _context.JobGrades
                .Include(jg => jg.JobPosition)
                .FirstOrDefaultAsync(jg => jg.Id == request.JobGradeId && jg.IsActive, cancellationToken);

            if (jobGrade == null)
                throw new InvalidOperationException("El puesto/nivel seleccionado no existe o está inactivo.");

            // Validar salario mínimo
            if (jobGrade.JobPosition.MinimumWageId.HasValue)
            {
                var minWage = await _context.MinimumWages
                    .FirstOrDefaultAsync(mw => mw.Id == jobGrade.JobPosition.MinimumWageId.Value, cancellationToken);

                if (minWage != null && request.BaseSalary < minWage.MonthlyAmountNIO)
                    throw new InvalidOperationException($"El salario debe ser mayor o igual al mínimo legal: C$ {minWage.MonthlyAmountNIO:N2}");
            }

            // ✅ ACTUALIZAR CAMPOS EDITABLES BÁSICOS
            employee.FirstName = request.FirstName;
            employee.LastName = request.LastName;
            employee.Email = request.Email;
            employee.Phone = request.Phone;
            employee.BranchId = request.BranchId;
            employee.ContractTypeId = request.ContractTypeId;
            employee.JobGradeId = request.JobGradeId;
            employee.BaseSalary = request.BaseSalary;
            employee.HealthProviderId = request.HealthProviderId;
            employee.BankId = request.BankId;
            employee.BankAccountNumber = request.BankAccountNumber;
            employee.BankAccountType = request.BankAccountType;
            employee.IsActive = request.IsActive;
            employee.OccupationalRiskId = jobGrade.JobPosition.OccupationalRiskId;

            // ✅ NUEVOS CAMPOS EDITABLES (Datos Fiscales)
            employee.NORUC = request.NORUC;
            employee.NOINSS = request.NOINSS;

            // ✅ NUEVOS CAMPOS EDITABLES (Condiciones Especiales)
            employee.IsTrustEmployee = request.IsTrustEmployee;

            // ✅ NUEVOS CAMPOS EDITABLES (Trabajador Extranjero)
            employee.Nationality = request.Nationality;
            employee.WorkPermitNumber = request.WorkPermitNumber;
            employee.WorkPermitExpirationDate = request.WorkPermitExpirationDate;

            // ✅ NUEVOS CAMPOS EDITABLES (Salario en Especie)
            employee.BenefitsInKindValue = request.BenefitsInKindValue;
            employee.BenefitsInKindDescription = request.BenefitsInKindDescription;

            // ✅ NUEVOS CAMPOS EDITABLES (Notas)
            employee.Notes = request.Notes;

            // ⚠️ NO SE MODIFICAN AQUÍ (manejados por Acciones de Personal o calculados automáticamente):
            // - EmploymentStatus
            // - SuspensionStartDate, SuspensionEndDate, SuspensionJustification, MitrabAuthorizationNumber
            // - PreviousEmployeeId
            // - ProbationStartDate, ProbationEndDate

            employee.CostCenterId = request.CostCenterId;

            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}