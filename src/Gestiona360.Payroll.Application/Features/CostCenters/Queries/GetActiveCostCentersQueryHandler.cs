using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.CostCenters.Queries
{
    public class GetActiveCostCentersQueryHandler : IRequestHandler<GetActiveCostCentersQuery, List<CostCenterDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetActiveCostCentersQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<List<CostCenterDto>> Handle(GetActiveCostCentersQuery request, CancellationToken cancellationToken)
        {
            return await _context.CostCenters
                .Where(c => c.IsActive)
                .OrderBy(c => c.Code)
                .Select(c => new CostCenterDto
                {
                    Id = c.Id,
                    Code = c.Code,
                    Name = c.Name,
                    CostType = c.CostType,
                    IsActive = c.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}
