using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface ICostCenterRepository
    {
        Task<IEnumerable<CostCenterBasicInfo>> GetActiveCostCentersAsync(CancellationToken ct = default);
        Task<CostCenter?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct = default);
        Task<bool> ExistsWithCodeExcludingAsync(string code, Guid excludeId, CancellationToken ct = default);
        Task<CostCenter> AddAsync(CostCenter costCenter, CancellationToken ct = default);
        void Update(CostCenter costCenter);
        void Delete(CostCenter costCenter);
    }
}
