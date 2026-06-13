using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public class GetEmployeeDetailQueryHandler : IRequestHandler<GetEmployeeDetailQuery, EmployeeDetailDto>
    {
        private readonly ApplicationDbContext _context;

        public GetEmployeeDetailQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<EmployeeDetailDto> Handle(GetEmployeeDetailQuery request, CancellationToken cancellationToken)
        {
            var employee = await _context.Employees
                .Include(e => e.Company)
                .Include(e => e.Branch)
                .Include(e => e.CostCenter)
                .Include(e => e.ContractType)
                .Include(e => e.JobGrade).ThenInclude(jg => jg!.JobPosition)
                .Include(e => e.Bank)
                .Include(e => e.HealthProvider)
                .Include(e => e.OccupationalRisk)
                .Include(e => e.PayrollGroup)  
                  .Include(e => e.Department)      
                .Include(e => e.Municipality)    
                 .ThenInclude(m => m.Department)
                .Include(e => e.ShiftAssignments)  
                    .ThenInclude(sa => sa.Shift)
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException($"Empleado con ID {request.EmployeeId} no encontrado.");

            // ✅ OBTENER TURNO ACTUAL
            var currentShiftAssignment = employee.ShiftAssignments?
                .FirstOrDefault(sa => sa.EndDate == null);

            return new EmployeeDetailDto
            {
                // DATOS BÁSICOS
                Id = employee.Id,
                Code = employee.Code,
                Identification = employee.Identification,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                Email = employee.Email,
                Phone = employee.Phone,
                HireDate = employee.HireDate,
                TerminationDate = employee.TerminationDate,
                IsActive = employee.IsActive,

                // IDs
                CompanyId = employee.CompanyId,
                BranchId = employee.BranchId,
                ContractTypeId = employee.ContractTypeId,
                JobGradeId = employee.JobGradeId,
                HealthProviderId = employee.HealthProviderId,
                BankId = employee.BankId,
                CostCenterId = employee.CostCenterId,
                OccupationalRiskId = employee.OccupationalRiskId, // ✅ AGREGADO
                PayrollGroupId = employee.PayrollGroupId, // ✅ NUEVO

                // NOMBRES
                CompanyName = employee.Company?.LegalName ?? "N/A",
                BranchName = employee.Branch?.Name ?? "N/A",
                BranchCode = employee.Branch?.Code ?? "N/A",
                ContractTypeName = employee.ContractType?.Name ?? "N/A",
                JobPositionName = employee.JobGrade?.JobPosition?.Name ?? "N/A",
                JobGradeName = employee.JobGrade?.Name ?? "N/A",
                HealthProviderName = employee.HealthProvider?.Name ?? "Sin proveedor",
                BankName = employee.Bank?.Name ?? "Sin cuenta",
                CostCenterName = employee.CostCenter?.Name ?? "No asignado",
                CostCenterCode = employee.CostCenter?.Code ?? "",
                OccupationalRiskName = employee.OccupationalRisk?.Name ?? "N/A",
                PayrollGroupName = employee.PayrollGroup?.Name ?? "No asignado", // ✅ NUEVO

                // FINANCIEROS
                BaseSalary = employee.BaseSalary,
                BankAccountNumber = employee.BankAccountNumber,
                BankAccountType = employee.BankAccountType,

                // IMÁGENES ✅ AGREGADO
                PhotoUrl = employee.PhotoUrl,
                IdFrontUrl = employee.IdFrontUrl,
                IdBackUrl = employee.IdBackUrl,

                // FISCALES
                NORUC = employee.NORUC,
                NOINSS = employee.NOINSS,

                // CONDICIONES ESPECIALES
                IsTrustEmployee = employee.IsTrustEmployee,
                BenefitsInKindValue = employee.BenefitsInKindValue,
                BenefitsInKindDescription = employee.BenefitsInKindDescription,

                // EXTRANJERO
                Nationality = employee.Nationality,
                WorkPermitNumber = employee.WorkPermitNumber,
                WorkPermitExpirationDate = employee.WorkPermitExpirationDate,

                // PERÍODO DE PRUEBA
                ProbationStartDate = employee.ProbationStartDate,
                ProbationEndDate = employee.ProbationEndDate,
                IsOnProbation = employee.ProbationStartDate.HasValue &&
                                (!employee.ProbationEndDate.HasValue || DateTime.UtcNow < employee.ProbationEndDate), // ✅ AGREGADO

                // ESTADO (convertido a string) ✅ CORREGIDO
                EmploymentStatus = employee.EmploymentStatus.ToString(),

                // SUSPENSIÓN ✅ AGREGADO
                SuspensionStartDate = employee.SuspensionStartDate,
                SuspensionEndDate = employee.SuspensionEndDate,
                SuspensionJustification = employee.SuspensionJustification,
                MitrabAuthorizationNumber = employee.MitrabAuthorizationNumber,

                // TURNO ACTUAL ✅ AGREGADO
                CurrentShiftName = currentShiftAssignment?.Shift?.Name,
                CurrentShiftSchedule = currentShiftAssignment?.Shift != null
                    ? $"{currentShiftAssignment.Shift.StartTime:hh\\:mm} - {currentShiftAssignment.Shift.EndTime:hh\\:mm}"
                    : null,
                ShiftAssignmentStartDate = currentShiftAssignment?.StartDate,

                // NOTAS Y REINGRESO
                Notes = employee.Notes,
                PreviousEmployeeId = employee.PreviousEmployeeId,

                SecondName = employee.SecondName,
                SecondLastName = employee.SecondLastName,
                BirthDate = employee.BirthDate,
                Gender = employee.Gender,
                MaritalStatus = employee.MaritalStatus,
                Address = employee.Address,
                DepartmentId = employee.DepartmentId,
                DepartmentName = employee.Department?.Name,
                MunicipalityId = employee.MunicipalityId,
                MunicipalityName = employee.Municipality?.Name,
                MobilePhone = employee.MobilePhone,
                EmergencyContactName = employee.EmergencyContactName,
                EmergencyContactPhone = employee.EmergencyContactPhone,
                EmergencyContactRelationship = employee.EmergencyContactRelationship,
                BankBeneficiaryName = employee.BankBeneficiaryName,
                FirstHireDate = employee.FirstHireDate,
                UsesTimeClock = employee.UsesTimeClock
            };
        }
    }
}
