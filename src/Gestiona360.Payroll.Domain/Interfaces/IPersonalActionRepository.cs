using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IPersonalActionRepository
    {
        Task<PersonalAction?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Employee>> GetEmployeesByIdsAsync(IEnumerable<Guid> employeeIds, CancellationToken ct = default);
        Task<PayrollPeriod?> GetPeriodByDateAndPayrollGroupAsync(Guid payrollGroupId, DateTime effectiveDate, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<PersonalAction> actions, CancellationToken ct = default);
        Task<IEnumerable<PersonalActionWithDetails>> GetPersonalActionsWithDetailsAsync(Guid employeeId, CancellationToken ct = default);
        Task AddAsync(PersonalAction action, CancellationToken ct = default);

        void Update(PersonalAction action);
        void Delete(PersonalAction action);  
        void DeleteRange(IEnumerable<PersonalAction> actions);

        Task<IEnumerable<PersonalAction>> GetPendingByBatchReferenceAsync(
     string batchReference,
     CancellationToken ct = default);

        Task<PersonalAction?> GetByIdWithEmployeeAsync(Guid id, CancellationToken ct = default);

        Task<bool> ExistsByBatchReferenceAsync(string batchReference, CancellationToken ct = default);

        Task<PersonalActionDetailDto?> GetDetailByIdAsync(Guid id, CancellationToken ct = default);
        Task<List<EmployeeBriefDto>> GetBatchEmployeesAsync(string batchReference, CancellationToken ct = default);
        Task<PersonalActionPagedResultDto> GetPagedAsync(PersonalActionFilterDto filter, CancellationToken ct = default);
    }
}
