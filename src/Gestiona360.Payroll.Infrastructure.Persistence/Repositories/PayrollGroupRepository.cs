using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class PayrollGroupRepository : IPayrollGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public PayrollGroupRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PayrollGroupBasicInfo>> GetActivePayrollGroupsAsync(CancellationToken ct = default)
        {
            return await _context.PayrollGroups
                .AsNoTracking()
                .Where(pg => pg.IsActive)
                .OrderBy(pg => pg.Name)
                .Select(pg => new PayrollGroupBasicInfo
                {
                    Id = pg.Id,
                    Name = pg.Name,
                    Code = pg.Code,
                    IsActive = pg.IsActive
                })
                .ToListAsync(ct);
        }

        public async Task<PayrollGroup?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.PayrollGroups.FindAsync(new object[] { id }, ct);
        }

        public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct = default)
        {
            return await _context.PayrollGroups
                .AsNoTracking()
                .AnyAsync(pg => pg.Code == code, ct);
        }

        public async Task<bool> ExistsWithCodeExcludingAsync(string code, Guid excludeId, CancellationToken ct = default)
        {
            return await _context.PayrollGroups
                .AsNoTracking()
                .AnyAsync(pg => pg.Code == code && pg.Id != excludeId, ct);
        }

        public async Task<PayrollGroup> AddAsync(PayrollGroup payrollGroup, CancellationToken ct = default)
        {
            await _context.PayrollGroups.AddAsync(payrollGroup, ct);
            return payrollGroup;
        }

        public void Update(PayrollGroup payrollGroup)
        {
            _context.PayrollGroups.Update(payrollGroup);
        }

        public void Delete(PayrollGroup payrollGroup)
        {
            _context.PayrollGroups.Remove(payrollGroup);
        }
    }
}
