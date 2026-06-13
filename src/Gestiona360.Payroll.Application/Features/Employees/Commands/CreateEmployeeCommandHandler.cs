using Gestiona360.Payroll.Application.Contracts.Common.Validators;
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

            // ═══════════════════════════════════════════════════════════════
            // VALIDACIONES ADICIONALES DE CAMPOS OBLIGATORIOS
            // ═══════════════════════════════════════════════════════════════
            if (!request.BirthDate.HasValue)
                throw new InvalidOperationException("La fecha de nacimiento es obligatoria.");

            if (string.IsNullOrWhiteSpace(request.MobilePhone))
                throw new InvalidOperationException("El número de celular es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.Address))
                throw new InvalidOperationException("La dirección del domicilio es obligatoria.");

            if (!request.DepartmentId.HasValue)
                throw new InvalidOperationException("El departamento es obligatorio.");

            if (!request.MunicipalityId.HasValue)
                throw new InvalidOperationException("El municipio es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.EmergencyContactName))
                throw new InvalidOperationException("El nombre del contacto de emergencia es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.EmergencyContactPhone))
                throw new InvalidOperationException("El teléfono del contacto de emergencia es obligatorio.");

            if (string.IsNullOrWhiteSpace(request.EmergencyContactRelationship))
                throw new InvalidOperationException("El parentesco del contacto de emergencia es obligatorio.");

            if (!CedulaValidator.EsValida(request.Identification))
                throw new InvalidOperationException("La cédula ingresada no es válida según el algoritmo de verificación nicaragüense.");

            // ═══════════════════════════════════════════════════════════════
            // VALIDACIÓN DE CÉDULA CON DETECCIÓN DE REINGRESO
            // ═══════════════════════════════════════════════════════════════
            Guid? previousEmployeeId = null;
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Identification == request.Identification, cancellationToken);

            if (existingEmployee != null)
            {
                if (existingEmployee.EmploymentStatus != EmploymentStatus.Terminated)
                {
                    throw new InvalidOperationException(
                        $"Ya existe un empleado activo/suspendido con la cédula {request.Identification}. " +
                        $"Solo se permite reingreso si el empleado anterior fue terminado/finiquitado.");
                }
                previousEmployeeId = existingEmployee.Id;
            }

            // ═══════════════════════════════════════════════════════════════
            // VALIDAR PUESTO/NIVEL
            // ═══════════════════════════════════════════════════════════════
            var jobGrade = await _context.JobGrades
                .Include(jg => jg.JobPosition)
                .FirstOrDefaultAsync(jg => jg.Id == request.JobGradeId && jg.IsActive, cancellationToken);

            if (jobGrade == null)
                throw new InvalidOperationException("El puesto/nivel seleccionado no existe o está inactivo");

            // ═══════════════════════════════════════════════════════════════
            // VALIDAR SALARIO MÍNIMO
            // ═══════════════════════════════════════════════════════════════
            if (jobGrade.JobPosition.MinimumWageId.HasValue)
            {
                var minWage = await _context.MinimumWages
                    .FirstOrDefaultAsync(mw => mw.Id == jobGrade.JobPosition.MinimumWageId.Value, cancellationToken);

                if (minWage != null && request.BaseSalary < minWage.MonthlyAmountNIO)
                    throw new InvalidOperationException($"El salario debe ser mayor o igual al mínimo legal: C$ {minWage.MonthlyAmountNIO:N2}");
            }

            // ═══════════════════════════════════════════════════════════════
            // VALIDAR DEPARTAMENTO Y MUNICIPIO
            // ═══════════════════════════════════════════════════════════════
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.Id == request.DepartmentId && d.IsActive, cancellationToken);
            if (department == null)
                throw new InvalidOperationException("El departamento seleccionado no existe o está inactivo.");

            var municipality = await _context.Municipalities
                .FirstOrDefaultAsync(m => m.Id == request.MunicipalityId && m.IsActive && m.DepartmentId == request.DepartmentId, cancellationToken);
            if (municipality == null)
                throw new InvalidOperationException("El municipio seleccionado no existe, está inactivo o no pertenece al departamento.");

            // ═══════════════════════════════════════════════════════════════
            // GENERAR CÓDIGO DE EMPLEADO
            // ═══════════════════════════════════════════════════════════════
            var employeeCode = await GenerateEmployeeCode(cancellationToken);

            // GENERAR Y GUARDAR CÓDIGO DE BARRAS
            // Asumiendo que tu entidad Employee tiene una propiedad llamada CodigoBarra o BarcodeData
            //var barcodePayload = _barcodeService.GeneratePayload(employeeCode, request.CompanyId);

            // ═══════════════════════════════════════════════════════════════
            // CÁLCULO AUTOMÁTICO DEL PERÍODO DE PRUEBA
            // ═══════════════════════════════════════════════════════════════
            DateTime? probationStartDate = null;
            DateTime? probationEndDate = null;

            var contractType = await _context.ContractTypes
                .FirstOrDefaultAsync(ct => ct.Id == request.ContractTypeId, cancellationToken);

            if (contractType != null && contractType.HasProbationPeriod)
            {
                probationStartDate = request.HireDate.Value;
                probationEndDate = request.HireDate.Value.AddDays(contractType.ProbationDays);
            }

            // ═══════════════════════════════════════════════════════════════
            // CREAR EMPLEADO CON TODOS LOS CAMPOS
            // ═══════════════════════════════════════════════════════════════
            var employee = new Employee
            {
                // Identificación
                Code = employeeCode,
                Identification = request.Identification,
                FirstName = request.FirstName,
                SecondName = request.SecondName,
                LastName = request.LastName,
                SecondLastName = request.SecondLastName,
                //CodigoBarra = barcodePayload,

                // Datos demográficos
                BirthDate = request.BirthDate,
                Gender = request.Gender,
                MaritalStatus = request.MaritalStatus,

                // Contacto
                Email = request.Email,
                Phone = request.Phone,
                MobilePhone = request.MobilePhone,

                // Domicilio
                Address = request.Address,
                DepartmentId = request.DepartmentId,
                MunicipalityId = request.MunicipalityId,

                // Contacto de emergencia
                EmergencyContactName = request.EmergencyContactName,
                EmergencyContactPhone = request.EmergencyContactPhone,
                EmergencyContactRelationship = request.EmergencyContactRelationship,

                // Datos fiscales
                NORUC = request.NORUC,
                NOINSS = request.NOINSS,

                // Contratación
                HireDate = request.HireDate.Value,
                FirstHireDate = request.FirstHireDate ?? request.HireDate.Value,
                CompanyId = request.CompanyId,
                BranchId = request.BranchId,
                ContractTypeId = request.ContractTypeId,
                JobGradeId = request.JobGradeId,
                HealthProviderId = request.HealthProviderId,
                OccupationalRiskId = jobGrade.JobPosition.OccupationalRiskId,
                BaseSalary = request.BaseSalary,
                CostCenterId = request.CostCenterId,
                PayrollGroupId = request.PayrollGroupId,

                // Datos bancarios
                BankId = request.BankId,
                BankAccountNumber = request.BankAccountNumber,
                BankAccountType = request.BankAccountType,
                BankBeneficiaryName = request.BankBeneficiaryName,

                // Condiciones especiales
                IsTrustEmployee = request.IsTrustEmployee,
                UsesTimeClock = request.UsesTimeClock,
                BenefitsInKindValue = request.BenefitsInKindValue,
                BenefitsInKindDescription = request.BenefitsInKindDescription,

                // Trabajador extranjero
                Nationality = request.Nationality,
                WorkPermitNumber = request.WorkPermitNumber,
                WorkPermitExpirationDate = request.WorkPermitExpirationDate,

                // Estado
                EmploymentStatus = EmploymentStatus.Active,
                IsActive = true,

                // Reingreso
                PreviousEmployeeId = previousEmployeeId,

                // Período de prueba
                ProbationStartDate = probationStartDate,
                ProbationEndDate = probationEndDate,

                // Notas
                Notes = request.Notes,

                // Auditoría
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