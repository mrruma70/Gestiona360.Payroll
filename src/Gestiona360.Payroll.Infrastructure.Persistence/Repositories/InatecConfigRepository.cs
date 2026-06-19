using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class InatecConfigRepository : IInatecConfigRepository
    {
        private readonly ApplicationDbContext _context;

        public InatecConfigRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<INATECConfig?> GetCurrentActiveConfigAsync(CancellationToken ct = default)
        {
            return await _context.INATECConfigs
                .Where(c => c.EffectiveTo == null && c.IsActive)
                .OrderByDescending(c => c.EffectiveFrom)
                .FirstOrDefaultAsync(ct);
        }

        public async Task CreateConfigAsync(INATECConfig config, CancellationToken ct = default)
        {
            await _context.INATECConfigs.AddAsync(config, ct);
        }

        public void UpdateConfig(INATECConfig config)
        {
            _context.INATECConfigs.Update(config);
        }
    }
}
