using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Companies.Queries
{
    public class GetAllBranchesQueryHandler : IRequestHandler<GetAllBranchesQuery, List<BranchDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetAllBranchesQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<List<BranchDto>> Handle(GetAllBranchesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Branches
      .Include(b => b.Manager)              // ✅ CAMBIAR de ManagerEmployee a Manager
      .Include(b => b.DefaultCostCenter)
      .Select(b => new BranchDto
      {
          Id = b.Id,
          Code = b.Code,
          Name = b.Name,
          Address = b.Address,
          City = b.City,
          Phone = b.Phone,
          CompanyId = b.CompanyId,
          IsActive = b.IsActive,
          ManagerEmployeeId = b.ManagerEmployeeId,
          DefaultCostCenterId = b.DefaultCostCenterId,
          IsZoneFranca = b.IsZoneFranca,
          // ✅ USAR Manager (como se llama en tu entidad)
          ManagerEmployeeName = b.Manager != null
              ? $"{b.Manager.FirstName} {b.Manager.LastName}"
              : null,
          CostCenterName = b.DefaultCostCenter != null
              ? b.DefaultCostCenter.Name
              : null
      })
      .OrderBy(b => b.Code)
      .ToListAsync(cancellationToken);
        }
    }
}
