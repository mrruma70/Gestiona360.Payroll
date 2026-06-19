// src/Gestiona360.Payroll.Infrastructure.Persistence/Repositories/BranchRepository.cs

using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class BranchRepository : IBranchRepository
{
    private readonly ApplicationDbContext _context;

    public BranchRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Branch?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await _context.Branches.FindAsync(new object[] { id }, ct);
    }

    public async Task<bool> ExistsWithCodeAsync(string code, CancellationToken ct = default)
    {
        return await _context.Branches
            .AsNoTracking()
            .AnyAsync(b => b.Code == code, ct);
    }

    public async Task<bool> ExistsWithCodeExcludingAsync(string code, Guid excludeId, CancellationToken ct = default)
    {
        return await _context.Branches
            .AsNoTracking()
            .AnyAsync(b => b.Code == code && b.Id != excludeId, ct);
    }

    // ✅ NUEVO: Verificar si la sucursal tiene empleados activos
    public async Task<bool> HasActiveEmployeesAsync(Guid branchId, CancellationToken ct = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .AnyAsync(e => e.BranchId == branchId && e.IsActive, ct);
    }

    public async Task<Branch> AddAsync(Branch branch, CancellationToken ct = default)
    {
        await _context.Branches.AddAsync(branch, ct);
        return branch;
    }

    public void Update(Branch branch)
    {
        _context.Branches.Update(branch);
    }

    public void Delete(Branch branch)
    {
        _context.Branches.Remove(branch);
    }

    /// <summary>
    /// Obtiene todas las sucursales con detalles del manager y centro de costo.
    /// </summary>
    public async Task<IEnumerable<BranchWithDetailsInfo>> GetAllWithDetailsAsync(CancellationToken ct = default)
    {
        return await _context.Branches
            .AsNoTracking()
            .Include(b => b.Manager)
            .Include(b => b.DefaultCostCenter)
            .OrderBy(b => b.Code)
            .Select(b => new BranchWithDetailsInfo
            {
                Id = b.Id,
                Code = b.Code,
                Name = b.Name,
                Address = b.Address,
                City = b.City,
                Phone = b.Phone,
                CompanyId = b.CompanyId,
                IsActive = b.IsActive,
                IsZoneFranca = b.IsZoneFranca,
                ManagerEmployeeId = b.ManagerEmployeeId,
                ManagerFirstName = b.Manager != null ? b.Manager.FirstName : null,
                ManagerLastName = b.Manager != null ? b.Manager.LastName : null,
                DefaultCostCenterId = b.DefaultCostCenterId,
                CostCenterName = b.DefaultCostCenter != null ? b.DefaultCostCenter.Name : null
            })
            .ToListAsync(ct);
    }
}