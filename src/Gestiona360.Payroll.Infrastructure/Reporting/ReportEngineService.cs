using System.Reflection;
using Dapper;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.Reports;
using Gestiona360.Payroll.Application.Contracts.Requests;
using Gestiona360.Payroll.Infrastructure.Reporting.Renderers;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Infrastructure.Reporting;

public class ReportEngineService : IReportEngine
{
    private readonly string _connectionString;
    private readonly IEnumerable<IReportRenderer> _renderers;
    private readonly ILogger<ReportEngineService> _logger;

    public ReportEngineService(
        IConfiguration configuration,
        IEnumerable<IReportRenderer> renderers,
        ILogger<ReportEngineService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'DefaultConnection'.");
        _renderers = renderers;
        _logger = logger;
    }

    public Task<IReadOnlyList<ReportDefinitionDto>> GetAvailableReportsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<ReportDefinitionDto>>(ReportCatalog.GetAll());

    public async Task<ReportResult> GenerateAsync(ReportRequest request, CancellationToken ct = default)
    {
        // 1. Validar que el reporte existe
        var definition = ReportCatalog.GetByCode(request.ReportCode)
            ?? throw new InvalidOperationException($"Reporte no encontrado: {request.ReportCode}");

        // 2. Validar formato
        var renderer = _renderers.FirstOrDefault(r => r.Format.Equals(request.Format, StringComparison.OrdinalIgnoreCase))
            ?? throw new InvalidOperationException($"Formato no soportado: {request.Format}");

        // 3. Validar parámetros requeridos
        ValidateParameters(definition, request.Parameters);

        // 4. Obtener SQL embebido
        var sqlFileName = $"{request.ReportCode}.sql";
        var sql = EmbeddedSqlReader.GetSql(sqlFileName, Assembly.GetExecutingAssembly());

        _logger.LogInformation("Ejecutando reporte {ReportCode} con formato {Format}", request.ReportCode, request.Format);

        // 5. Ejecutar consulta con Dapper
        // 5. Ejecutar consulta con Dapper
        IEnumerable<dynamic> data;
        await using (var connection = new SqlConnection(_connectionString))
        {
            // ✅ CORREGIDO: Agregar parámetros uno por uno
            var parameters = new DynamicParameters();

            foreach (var param in request.Parameters)
            {
                parameters.Add(param.Key, param.Value ?? DBNull.Value);
            }

            // TODO: Obtener CompanyId del contexto de seguridad
            // parameters.Add("CompanyId", _currentUser.CompanyId);

            data = await connection.QueryAsync(sql, parameters);
        }
        //IEnumerable<dynamic> data;
        //await using (var connection = new SqlConnection(_connectionString))
        //{
        //    // Inyectar parámetros de contexto (CompanyId del usuario autenticado)
        //    var parameters = new DynamicParameters(request.Parameters);
        //    // TODO: Obtener CompanyId del contexto de seguridad
        //    // parameters.Add("CompanyId", _currentUser.CompanyId);

        //    data = await connection.QueryAsync(sql, parameters);
        //}

        _logger.LogInformation("Reporte {ReportCode} devolvió {Count} registros", request.ReportCode, data.Count());

        // 6. Renderizar
        var content = await renderer.RenderAsync(definition.Name, data, ct);

        // 7. Construir nombre de archivo
        var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        var fileName = $"{request.ReportCode}_{timestamp}{renderer.FileExtension}";

        return new ReportResult(content, fileName, renderer.MimeType);
    }

    private static void ValidateParameters(ReportDefinitionDto definition, Dictionary<string, object> parameters)
    {
        foreach (var paramDef in definition.Parameters.Where(p => p.IsRequired))
        {
            if (!parameters.ContainsKey(paramDef.Name) || parameters[paramDef.Name] == null)
            {
                throw new InvalidOperationException(
                    $"El parámetro requerido '{paramDef.Label}' ({paramDef.Name}) no fue proporcionado.");
            }
        }
    }
}