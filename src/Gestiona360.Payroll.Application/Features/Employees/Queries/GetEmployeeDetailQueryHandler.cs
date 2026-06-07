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
                .FirstOrDefaultAsync(e => e.Id == request.EmployeeId, cancellationToken);

            if (employee == null)
                throw new KeyNotFoundException($"Empleado con ID {request.EmployeeId} no encontrado.");

            return new EmployeeDetailDto
            {
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

                CompanyName = employee.Company?.LegalName ?? "N/A",
                BranchName = employee.Branch?.Name ?? "N/A",
                BranchCode = employee.Branch?.Code ?? "N/A",
                ContractTypeName = employee.ContractType?.Name ?? "N/A",
                JobPositionName = employee.JobGrade?.JobPosition?.Name ?? "N/A",
                JobGradeName = employee.JobGrade?.Name ?? "N/A",
                BaseSalary = employee.BaseSalary,
                //BaseSalary = employee.JobGrade != null ? employee.JobGrade.BaseSalaryMultiplier * 1000m : 0, // Ajusta según tu lógica real de salario

                BankName = employee.Bank?.Name ?? "No asignado",
                BankAccountNumber = employee.BankAccountNumber ?? "N/A",
                BankAccountType = employee.BankAccountType ?? "N/A",

                OccupationalRiskName = employee.OccupationalRisk?.Name ?? "N/A",
                HealthProviderName = employee.HealthProvider?.Name ?? "N/A",

                CostCenterId = employee.CostCenterId,
                CostCenterName = employee.CostCenter?.Name ?? "No asignado",
                CostCenterCode = employee.CostCenter?.Code ?? ""
            };
        }
    }
}
