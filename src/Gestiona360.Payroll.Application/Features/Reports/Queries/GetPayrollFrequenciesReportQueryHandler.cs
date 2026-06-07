using Gestiona360.Payroll.Application.Contracts.Reports;
using Gestiona360.Payroll.Application.Contracts.Requests;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Reports.Queries;

public class GetPayrollFrequenciesReportQueryHandler : IRequestHandler<GetPayrollFrequenciesReportQuery, ReportResult>
{
    private readonly IReportEngine _reportEngine;

    public GetPayrollFrequenciesReportQueryHandler(IReportEngine reportEngine)
    {
        _reportEngine = reportEngine;
    }

    public async Task<ReportResult> Handle(GetPayrollFrequenciesReportQuery request, CancellationToken cancellationToken)
    {
        // ✅ CORRECTO: Crear el ReportRequest con los parámetros necesarios
        var reportRequest = new ReportRequest(
            ReportCode: "PAYROLL_FREQUENCIES",  // Debe coincidir con el nombre del archivo .sql
            Format: request.Format,
            Parameters: new Dictionary<string, object>()  // Sin parámetros adicionales si no son necesarios
        );

        // ✅ CORRECTO: Pasar el objeto ReportRequest completo
        return await _reportEngine.GenerateAsync(reportRequest, cancellationToken);
    }
}