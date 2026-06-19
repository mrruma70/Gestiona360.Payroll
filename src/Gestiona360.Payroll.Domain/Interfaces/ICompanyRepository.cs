using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface ICompanyRepository
    {
        Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);
        Task<bool> ExistsWithTaxIdAsync(string taxId, Guid excludeId, CancellationToken ct = default);

        Task<Company?> GetMainCompanyAsync(CancellationToken ct = default);
        void Update(Company company);
    }
}
