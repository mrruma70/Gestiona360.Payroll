using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Application.Services
{
    public interface IEmployeeExportService
    {
        Task<byte[]> ExportToExcelAsync(EmployeeExportFilters filters, CancellationToken ct = default);
    }
}
