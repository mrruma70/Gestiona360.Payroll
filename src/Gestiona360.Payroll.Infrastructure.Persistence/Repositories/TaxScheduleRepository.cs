using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class TaxScheduleRepository : ITaxScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public TaxScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IrTaxSchedule?> GetActiveScheduleByYearAsync(int year, CancellationToken ct = default)
        {
            return await _context.IrTaxSchedules
                .AsNoTracking()
                .Where(s => s.EffectiveFrom.Year == year && s.EffectiveTo == null)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<IrTaxSchedule> CreateScheduleAsync(IrTaxSchedule schedule, CancellationToken ct = default)
        {
            await _context.IrTaxSchedules.AddAsync(schedule, ct);
            await _context.SaveChangesAsync(ct);
            return schedule;
        }

        public async Task UpdateScheduleAsync(IrTaxSchedule schedule, CancellationToken ct = default)
        {
            _context.IrTaxSchedules.Update(schedule);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteBracketsByScheduleIdAsync(int scheduleId, CancellationToken ct = default)
        {
            var existingBrackets = await _context.IR_TaxBrackets
                .Where(b => b.ScheduleId == scheduleId)
                .ToListAsync(ct);

            _context.IR_TaxBrackets.RemoveRange(existingBrackets);
            await _context.SaveChangesAsync(ct);
        }

        public async Task AddBracketsAsync(IEnumerable<IR_TaxBracket> brackets, CancellationToken ct = default)
        {
            await _context.IR_TaxBrackets.AddRangeAsync(brackets, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> HasPayrollPeriodsForYearAsync(int year, CancellationToken ct = default)
        {
            return await _context.PayrollPeriods
                .AsNoTracking()
                .AnyAsync(p => p.StartDate.Year == year, ct);
        }

        public async Task<bool> HasClosedPayrollPeriodsForYearAsync(int year, CancellationToken ct = default)
        {
            return await _context.PayrollPeriods
                .AsNoTracking()
                .AnyAsync(p => p.StartDate.Year == year && p.Status == "Closed", ct);
        }
    }
}
