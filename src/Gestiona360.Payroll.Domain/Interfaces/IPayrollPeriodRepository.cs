using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IPayrollPeriodRepository
    {
        Task<PayrollPeriod?> GetPeriodByDateAndPayrollGroupAsync(
            Guid payrollGroupId,
            DateTime effectiveDate,
            CancellationToken ct = default);
    }
}
