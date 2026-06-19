using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class MunicipalityRepository : IMunicipalityRepository
{
    private readonly ApplicationDbContext _context;
    public MunicipalityRepository(ApplicationDbContext context) => _context = context;

    public async Task<Municipality?> GetByIdActiveInDepartmentAsync(int id, int departmentId, CancellationToken ct) // int según tu entidad
    {
        return await _context.Municipalities
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id && m.DepartmentId == departmentId && m.IsActive, ct);
    }
}