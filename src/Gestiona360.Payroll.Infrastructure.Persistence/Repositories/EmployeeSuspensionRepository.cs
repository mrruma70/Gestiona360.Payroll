using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{

    public class EmployeeSuspensionRepository : IEmployeeSuspensionRepository
    {
        private readonly ApplicationDbContext _context;
        public EmployeeSuspensionRepository(ApplicationDbContext context) => _context = context;


        public async Task<EmployeeSuspension?> GetActiveSuspensionByEmployeeIdAsync(Guid employeeId, CancellationToken ct = default)
        {
            return await _context.EmployeeSuspensions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.EndDate == null, ct);
        }
    }

}
