using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories
{
    public interface IPayrollRepository
    {
        // ... métodos existentes ...

        // ✅ NUEVO: Método específico para GetEmployeePayrollHistoryQuery
        Task<PayrollHistoryData?> GetPayrollHistoryAsync(Guid employeeId, CancellationToken ct = default);
    }
}
