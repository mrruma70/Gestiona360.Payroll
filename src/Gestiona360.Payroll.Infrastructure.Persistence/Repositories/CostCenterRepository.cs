using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class CostCenterRepository : ICostCenterRepository
    {
        private readonly ApplicationDbContext _context;

        public CostCenterRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CostCenterBasicInfo>> GetActiveCostCentersAsync(CancellationToken ct = default)
        {
            return await _context.CostCenters
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.Code)
                .Select(c => new CostCenterBasicInfo
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    CostType = c.CostType,
                    IsActive = c.IsActive
                })
                .ToListAsync(ct);
        }

        public async Task<CostCenter?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.CostCenters.FindAsync(new object[] { id }, ct);
        }

        public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct = default)
        {
            return await _context.CostCenters
                .AsNoTracking()
                .AnyAsync(c => c.Code == code, ct);
        }

        public async Task<bool> ExistsWithCodeExcludingAsync(string code, Guid excludeId, CancellationToken ct = default)
        {
            return await _context.CostCenters
                .AsNoTracking()
                .AnyAsync(c => c.Code == code && c.Id != excludeId, ct);
        }

        public async Task<CostCenter> AddAsync(CostCenter costCenter, CancellationToken ct = default)
        {
            await _context.CostCenters.AddAsync(costCenter, ct);
            return costCenter;
        }

        public void Update(CostCenter costCenter)
        {
            _context.CostCenters.Update(costCenter);
        }

        public void Delete(CostCenter costCenter)
        {
            _context.CostCenters.Remove(costCenter);
        }
    }
}
