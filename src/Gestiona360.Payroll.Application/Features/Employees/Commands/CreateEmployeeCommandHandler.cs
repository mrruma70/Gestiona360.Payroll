using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Guid>
    {
        private readonly ApplicationDbContext _context;

        public CreateEmployeeCommandHandler(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Guid> Handle(CreateEmployeeCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;

            if (!request.HireDate.HasValue)
                throw new InvalidOperationException("La fecha de ingreso es requerida.");

            // ✅ VALIDACIÓN DE CÉDULA CON DETECCIÓN DE REINGRESO
            Guid? previousEmployeeId = null;
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Identification == request.Identification, cancellationToken);

            if (existingEmployee != null)
            {
                // Si existe un empleado con la misma cédula...
                if (existingEmployee.EmploymentStatus != EmploymentStatus.Terminated)
                {
                    // ❌ No es reingreso: el empleado está activo o suspendido → duplicado real
                    throw new InvalidOperationException(
                        $"Ya existe un empleado activo/suspendido con la cédula {request.Identification}. " +
                        $"Solo se permite reingreso si el empleado anterior fue terminado/finiquitado.");
                }

                // ✅ Es un reingreso: el empleado anterior fue terminado
                previousEmployeeId = existingEmployee.Id;
            }

            // Validar que el JobGrade existe y está activo
            var jobGrade = await _context.JobGrades
                .Include(jg => jg.JobPosition)
                .FirstOrDefaultAsync(jg => jg.Id == request.JobGradeId && jg.IsActive, cancellationToken);

            if (jobGrade == null)
                throw new InvalidOperationException("El puesto/nivel seleccionado no existe o está inactivo");

            // Validar salario mínimo
            if (jobGrade.JobPosition.MinimumWageId.HasValue)
            {
                var minWage = await _context.MinimumWages
                    .FirstOrDefaultAsync(mw => mw.Id == jobGrade.JobPosition.MinimumWageId.Value, cancellationToken);

                if (minWage != null && request.BaseSalary < minWage.MonthlyAmountNIO)
                    throw new InvalidOperationException($"El salario debe ser mayor o igual al mínimo legal: C$ {minWage.MonthlyAmountNIO:N2}");
            }

            // Generar código de empleado
            var employeeCode = await GenerateEmployeeCode(cancellationToken);

            // ✅ CÁLCULO AUTOMÁTICO DEL PERÍODO DE PRUEBA (Art. 27 Ley 185)
            DateTime? probationStartDate = null;
            DateTime? probationEndDate = null;

            var contractType = await _context.ContractTypes
                .FirstOrDefaultAsync(ct => ct.Id == request.ContractTypeId, cancellationToken);

            if (contractType != null && contractType.HasProbationPeriod)
            {
                probationStartDate = request.HireDate.Value;
                probationEndDate = request.HireDate.Value.AddDays(contractType.ProbationDays);
            }

            // ✅ CREAR EMPLEADO CON TODOS LOS CAMPOS
            var employee = new Employee
            {
                // Campos básicos
                Code = employeeCode,
                Identification = request.Identification,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                HireDate = request.HireDate.Value,
                CompanyId = request.CompanyId,
                BranchId = request.BranchId,
                ContractTypeId = request.ContractTypeId,
                JobGradeId = request.JobGradeId,
                HealthProviderId = request.HealthProviderId,
                OccupationalRiskId = jobGrade.JobPosition.OccupationalRiskId,
                BaseSalary = request.BaseSalary,
                BankId = request.BankId,
                BankAccountNumber = request.BankAccountNumber,
                BankAccountType = request.BankAccountType,

                // ✅ NUEVOS CAMPOS EDITABLES
                NORUC = request.NORUC,
                NOINSS = request.NOINSS,
                IsTrustEmployee = request.IsTrustEmployee,
                Nationality = request.Nationality,
                WorkPermitNumber = request.WorkPermitNumber,
                WorkPermitExpirationDate = request.WorkPermitExpirationDate,
                BenefitsInKindValue = request.BenefitsInKindValue,
                BenefitsInKindDescription = request.BenefitsInKindDescription,
                Notes = request.Notes,

                // ✅ ESTADO INICIAL (siempre Active al crear)
                EmploymentStatus = EmploymentStatus.Active,

                // ✅ REINGRESO (si aplica)
                PreviousEmployeeId = previousEmployeeId,

                // ✅ PERÍODO DE PRUEBA (calculado automáticamente)
                ProbationStartDate = probationStartDate,
                ProbationEndDate = probationEndDate,

                CostCenterId = request.CostCenterId,

                // ✅ CONTROL
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync(cancellationToken);

            return employee.Id;
        }

        private async Task<string> GenerateEmployeeCode(CancellationToken cancellationToken)
        {
            var lastEmployee = await _context.Employees
                .OrderByDescending(e => e.Code)
                .FirstOrDefaultAsync(cancellationToken);

            if (lastEmployee == null)
                return "EMP-001";

            var lastNumber = lastEmployee.Code.Replace("EMP-", "");
            if (int.TryParse(lastNumber, out int number))
                return $"EMP-{(number + 1).ToString().PadLeft(3, '0')}";

            return $"EMP-{DateTime.UtcNow.Ticks.ToString().Substring(0, 3)}";
        }
    }
}
