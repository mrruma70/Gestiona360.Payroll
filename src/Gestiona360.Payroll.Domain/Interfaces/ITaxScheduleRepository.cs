using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface ITaxScheduleRepository
    {
        Task<IrTaxSchedule?> GetActiveScheduleByYearAsync(int year, CancellationToken ct = default);
        Task<IrTaxSchedule> CreateScheduleAsync(IrTaxSchedule schedule, CancellationToken ct = default);
        Task UpdateScheduleAsync(IrTaxSchedule schedule, CancellationToken ct = default);
        Task DeleteBracketsByScheduleIdAsync(int scheduleId, CancellationToken ct = default);
        Task AddBracketsAsync(IEnumerable<IR_TaxBracket> brackets, CancellationToken ct = default);
        Task<bool> HasPayrollPeriodsForYearAsync(int year, CancellationToken ct = default);
        Task<bool> HasClosedPayrollPeriodsForYearAsync(int year, CancellationToken ct = default);
    }
}
