using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class JobGradeRepository : IJobGradeRepository
{
    private readonly ApplicationDbContext _context;
    public JobGradeRepository(ApplicationDbContext context) => _context = context;


    public async Task<JobGrade?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.JobGrades
            .AsNoTracking()
            .FirstOrDefaultAsync(jg => jg.Id == id, ct);
    }


    public async Task<JobGrade?> GetByIdWithPositionAsync(Guid id, CancellationToken ct)
    {
        return await _context.JobGrades
            .Include(jg => jg.JobPosition)
            .FirstOrDefaultAsync(jg => jg.Id == id && jg.IsActive, ct);
    }

    public async Task<JobGrade?> GetByIdWithPositionDetailsAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.JobGrades
            .Include(jg => jg.JobPosition)
                .ThenInclude(jp => jp.MinimumWage)
            .AsNoTracking()
            .FirstOrDefaultAsync(jg => jg.Id == id, ct);
    }


}
