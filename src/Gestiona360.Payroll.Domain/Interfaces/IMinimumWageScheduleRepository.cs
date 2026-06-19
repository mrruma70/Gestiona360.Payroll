using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IMinimumWageScheduleRepository
    {
        Task<MinimumWageSchedule?> GetActiveScheduleByYearAsync(int year, CancellationToken ct = default);
        Task<MinimumWageSchedule> CreateScheduleAsync(MinimumWageSchedule schedule, CancellationToken ct = default);
        Task UpdateScheduleAsync(MinimumWageSchedule schedule, CancellationToken ct = default);
        Task DeleteWagesByScheduleIdAsync(int scheduleId, CancellationToken ct = default);
        Task<MinimumWage?> GetByScheduleAndSectorAsync(int scheduleId, string sector, CancellationToken ct = default);
        Task AddWageAsync(MinimumWage wage, CancellationToken ct = default);
        Task UpdateWageAsync(MinimumWage wage, CancellationToken ct = default);
        Task AddWagesAsync(IEnumerable<MinimumWage> wages, CancellationToken ct = default);
        Task<bool> HasPayrollPeriodsForYearAsync(int year, CancellationToken ct = default);
        Task<bool> HasClosedPayrollPeriodsForYearAsync(int year, CancellationToken ct = default);
    }
}
