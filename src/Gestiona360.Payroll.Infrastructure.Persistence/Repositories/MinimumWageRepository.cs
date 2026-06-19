using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class MinimumWageRepository : IMinimumWageRepository
{
    private readonly ApplicationDbContext _context;
    public MinimumWageRepository(ApplicationDbContext context) => _context = context;

    public async Task<MinimumWage?> GetByIdAsync(int id, CancellationToken ct) // Ajusta a Guid si tu entidad usa Guid
    {
        return await _context.MinimumWages
            .AsNoTracking()
            .FirstOrDefaultAsync(mw => mw.Id == id, ct);
    }
}