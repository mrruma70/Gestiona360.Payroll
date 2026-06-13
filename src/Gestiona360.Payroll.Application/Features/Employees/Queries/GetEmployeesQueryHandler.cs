using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, EmployeeSearchResultDto>
    {
        private readonly ApplicationDbContext _context;

        public GetEmployeesQueryHandler(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<EmployeeSearchResultDto> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Employees
                .Include(e => e.Branch)
                .Include(e => e.CostCenter)
                .Include(e => e.JobGrade)
                    .ThenInclude(jg => jg!.JobPosition)
                .Include(e => e.ContractType)
                .Include(e => e.Bank)
                .Include(e => e.HealthProvider)
                .Include(e => e.PayrollGroup)  
                .Include(e => e.ShiftAssignments)
                    .ThenInclude(sa => sa.Shift)
                .AsQueryable();

            // Filtros existentes
            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                var searchTerm = request.Search.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(searchTerm) ||
                    e.LastName.ToLower().Contains(searchTerm) ||
                    e.Identification.ToLower().Contains(searchTerm) ||
                    e.Code.ToLower().Contains(searchTerm) ||
                    e.Email.ToLower().Contains(searchTerm) ||
                    e.NORUC.ToLower().Contains(searchTerm) ||
                    e.NOINSS.ToLower().Contains(searchTerm));
            }

            if (request.BranchId.HasValue)
                query = query.Where(e => e.BranchId == request.BranchId.Value);

            if (request.ContractTypeId.HasValue)
                query = query.Where(e => e.ContractTypeId == request.ContractTypeId.Value);

            if (!string.IsNullOrEmpty(request.Status))
            {
                var isActive = request.Status.ToLower() == "active";
                query = query.Where(e => e.IsActive == isActive);
            }

            if (request.EmploymentStatus.HasValue)
                query = query.Where(e => e.EmploymentStatus == (EmploymentStatus)request.EmploymentStatus.Value);

            if (request.JobPositionId.HasValue)
                query = query.Where(e => e.JobGrade != null && e.JobGrade.JobPositionId == request.JobPositionId.Value);

            if (request.IsTrustEmployee.HasValue)
                query = query.Where(e => e.IsTrustEmployee == request.IsTrustEmployee.Value);

            if (request.IsForeignWorker.HasValue)
            {
                if (request.IsForeignWorker.Value)
                    query = query.Where(e => !string.IsNullOrEmpty(e.Nationality));
                else
                    query = query.Where(e => string.IsNullOrEmpty(e.Nationality));
            }

            if (request.IsOnProbation.HasValue)
            {
                var now = DateTime.UtcNow;
                if (request.IsOnProbation.Value)
                    query = query.Where(e => e.ProbationStartDate.HasValue &&
                                            e.ProbationEndDate.HasValue &&
                                            now >= e.ProbationStartDate.Value &&
                                            now <= e.ProbationEndDate.Value);
                else
                    query = query.Where(e => !e.ProbationStartDate.HasValue ||
                                            !e.ProbationEndDate.HasValue ||
                                            now < e.ProbationStartDate.Value ||
                                            now > e.ProbationEndDate.Value);
            }

            if (request.IsRehire.HasValue)
            {
                if (request.IsRehire.Value)
                    query = query.Where(e => e.PreviousEmployeeId.HasValue);
                else
                    query = query.Where(e => !e.PreviousEmployeeId.HasValue);
            }

            // ✅ NUEVO: Filtro por Grupo de Nómina
            if (request.PayrollGroupId.HasValue)
                query = query.Where(e => e.PayrollGroupId == request.PayrollGroupId.Value);

            var now2 = DateTime.UtcNow;
            var employees = await query
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new EmployeeListDto
                {
                    Id = e.Id,
                    Code = e.Code,
                    Identification = e.Identification,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Email = e.Email,
                    Phone = e.Phone,
                    HireDate = e.HireDate,
                    IsActive = e.IsActive,
                    JobPositionName = e.JobGrade != null ? e.JobGrade.JobPosition.Name : "Sin puesto",
                    JobGradeName = e.JobGrade != null ? e.JobGrade.Name : "",
                    BranchName = e.Branch != null ? e.Branch.Name : "",
                    BranchCode = e.Branch != null ? e.Branch.Code : "",
                    ContractTypeName = e.ContractType != null ? e.ContractType.Name : "",
                    BaseSalary = e.BaseSalary,
                    EmploymentStatus = e.EmploymentStatus,
                    IsTrustEmployee = e.IsTrustEmployee,
                    IsOnProbation = e.ProbationStartDate.HasValue &&
                                   e.ProbationEndDate.HasValue &&
                                   now2 >= e.ProbationStartDate.Value &&
                                   now2 <= e.ProbationEndDate.Value,
                    Nationality = e.Nationality,
                    ProbationEndDate = e.ProbationEndDate,
                    CostCenterId = e.CostCenterId,
                    CostCenterName = e.CostCenter != null ? e.CostCenter.Name : "",
                    CostCenterCode = e.CostCenter != null ? e.CostCenter.Code : "",
                    BankId = e.BankId,
                    BankName = e.Bank != null ? e.Bank.Name : "",
                    BankAccountNumber = e.BankAccountNumber ?? "",
                    HealthProviderId = e.HealthProviderId,
                    HealthProviderName = e.HealthProvider != null ? e.HealthProvider.Name : "",
                    ShiftName = e.ShiftAssignments
                        .FirstOrDefault(sa => sa.EndDate == null) != null
                        ? e.ShiftAssignments.First(sa => sa.EndDate == null).Shift.Name
                        : "Sin turno asignado",
                    PreviousEmployeeId = e.PreviousEmployeeId,

                   
                    PayrollGroupId = e.PayrollGroupId,
                    PayrollGroupName = e.PayrollGroup != null ? e.PayrollGroup.Name : "No asignado"
                })
                .ToListAsync(cancellationToken);

            var allEmployees = _context.Employees;
            var stats = new EmployeeStatsDto
            {
                Total = await allEmployees.CountAsync(cancellationToken),
                Active = await allEmployees.CountAsync(e => e.IsActive, cancellationToken),
                Inactive = await allEmployees.CountAsync(e => !e.IsActive, cancellationToken),
                NewThisMonth = await allEmployees.CountAsync(e =>
                    e.HireDate.Month == now2.Month && e.HireDate.Year == now2.Year, cancellationToken),
                Suspended = await allEmployees.CountAsync(e =>
                    e.EmploymentStatus == EmploymentStatus.Suspended, cancellationToken),
                Terminated = await allEmployees.CountAsync(e =>
                    e.EmploymentStatus == EmploymentStatus.Terminated, cancellationToken),
                OnProbation = await allEmployees.CountAsync(e =>
                    e.ProbationStartDate.HasValue &&
                    e.ProbationEndDate.HasValue &&
                    now2 >= e.ProbationStartDate.Value &&
                    now2 <= e.ProbationEndDate.Value, cancellationToken),
                TrustEmployees = await allEmployees.CountAsync(e => e.IsTrustEmployee, cancellationToken),
                ForeignWorkers = await allEmployees.CountAsync(e => !string.IsNullOrEmpty(e.Nationality), cancellationToken),
                Rehires = await allEmployees.CountAsync(e => e.PreviousEmployeeId.HasValue, cancellationToken)
            };

            return new EmployeeSearchResultDto
            {
                Employees = employees,
                Stats = stats
            };
        }
    }
}
