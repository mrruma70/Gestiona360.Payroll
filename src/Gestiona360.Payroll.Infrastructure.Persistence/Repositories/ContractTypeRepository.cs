using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class ContractTypeRepository : IContractTypeRepository
{
    private readonly ApplicationDbContext _context;
    public ContractTypeRepository(ApplicationDbContext context) => _context = context;

    public async Task<ContractType?> GetByIdAsync(int id, CancellationToken ct) // int según tu entidad Employee
    {
        return await _context.ContractTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(ct => ct.Id == id, ct);
    }
}