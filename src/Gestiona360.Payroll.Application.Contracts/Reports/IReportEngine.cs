using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.Requests;

namespace Gestiona360.Payroll.Application.Contracts.Reports
{
    /// <summary>
    /// Contrato principal del motor de reportes.
    /// </summary>
    public interface IReportEngine
    {
        Task<ReportResult> GenerateAsync(ReportRequest request, CancellationToken ct = default);
        Task<IReadOnlyList<ReportDefinitionDto>> GetAvailableReportsAsync(CancellationToken ct = default);
    }
}
