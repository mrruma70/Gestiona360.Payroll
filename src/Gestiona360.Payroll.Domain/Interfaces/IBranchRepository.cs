using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IBranchRepository
    {
        Task<Branch?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct = default);
        Task<bool> ExistsWithCodeExcludingAsync(string code, Guid excludeId, CancellationToken ct = default); // ✅ NUEVO
        Task<Branch> AddAsync(Branch branch, CancellationToken ct = default);
        Task<bool> HasActiveEmployeesAsync(Guid branchId, CancellationToken ct = default);

        Task<IEnumerable<BranchWithDetailsInfo>> GetAllWithDetailsAsync(CancellationToken ct = default);
        void Update(Branch branch);
        void Delete(Branch branch);
    }
}
