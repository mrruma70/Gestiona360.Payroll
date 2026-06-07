using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public class GetActiveEmployeesQueryHandler : IRequestHandler<GetActiveEmployeesQuery, List<EmployeeDto>>
    {
        private readonly ApplicationDbContext _context;

        public GetActiveEmployeesQueryHandler(ApplicationDbContext context) => _context = context;

        public async Task<List<EmployeeDto>> Handle(GetActiveEmployeesQuery request, CancellationToken cancellationToken)
        {
            return await _context.Employees
                .Where(e => e.IsActive)
                .OrderBy(e => e.FirstName)
                .Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FullName = $"{e.FirstName} {e.LastName}",
                    Identification = e.Identification,
                    Email = e.Email,
                    Phone = e.Phone,
                    IsActive = e.IsActive
                })
                .ToListAsync(cancellationToken);
        }
    }
}
