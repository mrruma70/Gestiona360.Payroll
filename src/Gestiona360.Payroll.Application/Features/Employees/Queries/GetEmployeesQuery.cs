using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public record GetEmployeesQuery(
        string? Search = null,
        Guid? BranchId = null,
        int? ContractTypeId = null,
        string? Status = null,
        Guid? JobPositionId = null,  
        int? EmploymentStatus = null,
        bool? IsTrustEmployee = null,
        bool? IsForeignWorker = null,
        bool? IsOnProbation = null,
        bool? IsRehire = null,
        Guid? PayrollGroupId = null
    ) : IRequest<EmployeeSearchResultDto>;
}
