using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class PayrollPeriodRepository : IPayrollPeriodRepository
    {
        private readonly ApplicationDbContext _context;

        public PayrollPeriodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PayrollPeriod?> GetPeriodByDateAndPayrollGroupAsync(
            Guid payrollGroupId,
            DateTime effectiveDate,
            CancellationToken ct = default)
        {
            return await _context.PayrollPeriods
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PayrollGroupId == payrollGroupId &&
                                          effectiveDate >= p.StartDate &&
                                          effectiveDate <= p.EndDate, ct);
        }
    }
}
