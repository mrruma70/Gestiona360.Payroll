using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Gestiona360.Payroll.Application.Features.PayrollGroups.Queries
{
    public class GetActivePayrollGroupsQueryHandler
        : IRequestHandler<GetActivePayrollGroupsQuery, List<PayrollGroupDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetActivePayrollGroupsQueryHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PayrollGroupDto>> Handle(
            GetActivePayrollGroupsQuery request,
            CancellationToken cancellationToken)
        {
            return await _context.PayrollGroups
                .Where(pg => pg.IsActive)
                .OrderBy(pg => pg.Name)
                .Select(pg => new PayrollGroupDto
                {
                    Id = pg.Id,
                    Name = pg.Name,
                    Code = pg.Code,
                    IsActive = pg.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}