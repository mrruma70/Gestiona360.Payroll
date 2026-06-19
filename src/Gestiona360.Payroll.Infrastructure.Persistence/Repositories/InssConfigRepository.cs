using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class InssConfigRepository : IInssConfigRepository
    {
        private readonly ApplicationDbContext _context;

        public InssConfigRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<INSSConfig?> GetCurrentActiveConfigAsync(CancellationToken ct = default)
        {
            return await _context.INSSConfigs
                .Where(c => c.EffectiveTo == null && c.IsActive)
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync(ct);
        }

        public async Task CreateConfigAsync(INSSConfig config, CancellationToken ct = default)
        {
            await _context.INSSConfigs.AddAsync(config, ct);
        }

        public void UpdateConfig(INSSConfig config)
        {
            _context.INSSConfigs.Update(config);
        }
    }
}
