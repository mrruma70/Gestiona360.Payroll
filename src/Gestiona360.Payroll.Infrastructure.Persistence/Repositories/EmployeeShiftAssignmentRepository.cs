using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class EmployeeShiftAssignmentRepository : IEmployeeShiftAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeShiftAssignmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeShiftAssignment?> GetActiveShiftByEmployeeIdAsync(
            Guid employeeId,
            CancellationToken ct = default)
        {
            return await _context.EmployeeShiftAssignments
                .AsNoTracking()
                .FirstOrDefaultAsync(sa => sa.EmployeeId == employeeId && sa.EndDate == null, ct);
        }
    }
}
