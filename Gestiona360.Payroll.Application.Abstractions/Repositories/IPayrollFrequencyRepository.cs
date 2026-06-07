using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Application.Abstractions.Repositories
{
    public interface IPayrollFrequencyRepository
    {
        Task<List<PayrollFrequency>> GetAllActiveAsync(CancellationToken ct = default);
        Task<PayrollFrequency?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(PayrollFrequency entity, CancellationToken ct = default);
        void Update(PayrollFrequency entity);

        Task<(List<PayrollFrequencyDto> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? sort,
        string? dir,
        CancellationToken cancellationToken);
    }
}
