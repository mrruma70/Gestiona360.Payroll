using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class PayrollFrequencyRepository : IPayrollFrequencyRepository
{
    private readonly ApplicationDbContext _context;

    public PayrollFrequencyRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PayrollFrequency>> GetAllActiveAsync(CancellationToken ct = default)
    {
        return await _context.PayrollFrequencies
            .Where(f => f.IsActive)
            .OrderBy(f => f.Name)
            .AsNoTracking()
            .ToListAsync(ct);
    }

    public async Task<PayrollFrequency?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.PayrollFrequencies
    .FirstOrDefaultAsync(x => x.Id == id, ct);
    }

    public async Task AddAsync(PayrollFrequency entity, CancellationToken ct = default)
    {
        await _context.PayrollFrequencies.AddAsync(entity, ct);
    }

    public void Update(PayrollFrequency entity)
    {
        _context.PayrollFrequencies.Update(entity);
    }

    public async Task<(List<PayrollFrequencyDto> Items, int Total)> GetPagedAsync(
        int page,
        int pageSize,
        string? sort,
        string? dir,
        CancellationToken cancellationToken)
    {
        var query = _context.PayrollFrequencies.AsNoTracking();

        query = sort switch
        {
            "Name" => dir == "desc"
                ? query.OrderByDescending(x => x.Name)
                : query.OrderBy(x => x.Name),

            "Code" => dir == "desc"
                ? query.OrderByDescending(x => x.Code)
                : query.OrderBy(x => x.Code),

            _ => query
        };

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(page * pageSize)
            .Take(pageSize)
            .Select(x => new PayrollFrequencyDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                DaysPerPeriod = x.DaysPerPeriod,
                PeriodsPerYear = x.PeriodsPerYear,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}