using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IPayrollGroupRepository
    {
        Task<IEnumerable<PayrollGroupBasicInfo>> GetActivePayrollGroupsAsync(CancellationToken ct = default);
        Task<PayrollGroup?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct = default);
        Task<bool> ExistsWithCodeExcludingAsync(string code, Guid excludeId, CancellationToken ct = default);
        Task<PayrollGroup> AddAsync(PayrollGroup payrollGroup, CancellationToken ct = default);
        void Update(PayrollGroup payrollGroup);
        void Delete(PayrollGroup payrollGroup);
    }
}
