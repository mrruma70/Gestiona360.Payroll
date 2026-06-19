using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Company?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Companies.FindAsync(new object[] { id }, ct);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Companies
                .AsNoTracking()
                .AnyAsync(c => c.Id == id, ct);
        }

        public async Task<bool> ExistsWithTaxIdAsync(string taxId, Guid excludeId, CancellationToken ct = default)
        {
            return await _context.Companies
                .AsNoTracking()
                .AnyAsync(c => c.TaxId == taxId && c.Id != excludeId, ct);
        }

        /// <summary>
        /// Obtiene la empresa principal (primera empresa activa ordenada por fecha de creación).
        /// Asume modelo monoempresa o retorna la más antigua si hay varias.
        /// </summary>
        public async Task<Company?> GetMainCompanyAsync(CancellationToken ct = default)
        {
            return await _context.Companies
                .AsNoTracking()
                .Where(c => c.IsActive)
                .OrderBy(c => c.CreatedAt)
                .FirstOrDefaultAsync(ct);
        }


        public void Update(Company company)
        {
            _context.Companies.Update(company);
        }
    }
}
