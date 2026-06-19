using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly ApplicationDbContext _context;
    public DepartmentRepository(ApplicationDbContext context) => _context = context;

    public async Task<Department?> GetByIdActiveAsync(int id, CancellationToken ct) // int según tu entidad Employee
    {
        return await _context.Departments
            .AsNoTracking()
            .FirstOrDefaultAsync(d => d.Id == id && d.IsActive, ct);
    }
}