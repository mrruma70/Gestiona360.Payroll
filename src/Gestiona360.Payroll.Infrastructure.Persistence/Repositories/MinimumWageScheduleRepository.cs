using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public class MinimumWageScheduleRepository : IMinimumWageScheduleRepository
    {
        private readonly ApplicationDbContext _context;

        public MinimumWageScheduleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<MinimumWageSchedule?> GetActiveScheduleByYearAsync(int year, CancellationToken ct = default)
        {
            return await _context.MinimumWageSchedules
                .AsNoTracking()
                .Where(s => s.EffectiveFrom.Year == year && s.EffectiveTo == null)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<MinimumWageSchedule> CreateScheduleAsync(MinimumWageSchedule schedule, CancellationToken ct = default)
        {
            await _context.MinimumWageSchedules.AddAsync(schedule, ct);
            await _context.SaveChangesAsync(ct);
            return schedule;
        }

        public async Task UpdateScheduleAsync(MinimumWageSchedule schedule, CancellationToken ct = default)
        {
            _context.MinimumWageSchedules.Update(schedule);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteWagesByScheduleIdAsync(int scheduleId, CancellationToken ct = default)
        {
            var existingWages = await _context.MinimumWages
                .Where(w => w.ScheduleId == scheduleId)
                .ToListAsync(ct);

            _context.MinimumWages.RemoveRange(existingWages);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<MinimumWage?> GetByScheduleAndSectorAsync(int scheduleId, string sector, CancellationToken ct = default)
        {
            return await _context.MinimumWages
                .FirstOrDefaultAsync(w => w.ScheduleId == scheduleId && w.Sector == sector, ct);
        }

        public async Task AddWageAsync(MinimumWage wage, CancellationToken ct = default)
        {
            await _context.MinimumWages.AddAsync(wage, ct);
        }

        public Task UpdateWageAsync(MinimumWage wage, CancellationToken ct = default)
        {
            _context.MinimumWages.Update(wage);
            return Task.CompletedTask;
        }

        public async Task AddWagesAsync(IEnumerable<MinimumWage> wages, CancellationToken ct = default)
        {
            await _context.MinimumWages.AddRangeAsync(wages, ct);
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
