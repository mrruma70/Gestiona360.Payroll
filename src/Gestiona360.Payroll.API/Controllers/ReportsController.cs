using Gestiona360.Payroll.Application.Contracts.Reports;
using Gestiona360.Payroll.Application.Contracts.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //[Authorize] // ✅ Mantiene la seguridad a nivel de clase
    public class ReportsController : ControllerBase
    {
        private readonly IReportEngine _reportEngine;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IReportEngine reportEngine, ILogger<ReportsController> logger)
        {
            _reportEngine = reportEngine;
            _logger = logger;
        }

        [HttpGet("catalog")]
        public async Task<IActionResult> GetCatalog(CancellationToken ct)
        {
            var reports = await _reportEngine.GetAvailableReportsAsync(ct);
            return Ok(reports);
        }


        /// <summary>
        /// POST: api/reports/generate
        /// Genera y descarga un reporte.
        /// </summary>
        [HttpPost("generate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Generate([FromBody] ReportRequest request, CancellationToken ct)
        {
            try
            {
                var result = await _reportEngine.GenerateAsync(request, ct);
                return File(result.Content, result.MimeType, result.FileName);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Error de validación generando reporte");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error crítico generando reporte {Code}", request.ReportCode);
                return StatusCode(500, new { error = "Error interno generando el reporte.", detail = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/reports/payroll-frequencies?format=Excel
        /// Genera un reporte de Frecuencias de Nómina.
        /// </summary>
        [HttpGet("payroll-frequencies")]
        public async Task<IActionResult> GetPayrollFrequenciesReport([FromQuery] string format = "Excel")
        {
            try
            {
                var allowedFormats = new[] { "Excel", "Csv", "Pdf" };
                if (!allowedFormats.Contains(format, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest($"Formato no soportado. Use: {string.Join(", ", allowedFormats)}");
                }

                // ✅ Usar el motor de reportes directamente
                var request = new ReportRequest(
                    ReportCode: "PAYROLL_FREQUENCIES",
                    Format: format,
                    Parameters: new Dictionary<string, object>()
                );

                var result = await _reportEngine.GenerateAsync(request, HttpContext.RequestAborted);

                return File(result.Content, result.MimeType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando reporte de frecuencias");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/reports/company-legal-sheet
        /// Genera la ficha legal de la empresa en PDF.
        /// </summary>
        //[HttpGet("company-legal-sheet")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetCompanyLegalSheet([FromQuery] string format = "Pdf")
        //{
        //    try
        //    {
        //        var request = new ReportRequest(
        //            ReportCode: "COMPANY_LEGAL_SHEET",
        //            Format: format,
        //            Parameters: new Dictionary<string, object>()
        //        );

        //        var result = await _reportEngine.GenerateAsync(request, HttpContext.RequestAborted);
        //        return File(result.Content, result.MimeType, result.FileName);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error generando ficha legal");
        //        return StatusCode(500, new { error = ex.Message });
        //    }
        //}

        /// <summary>
        /// GET: api/reports/branches-list
        /// Genera el listado de sucursales en Excel.
        /// </summary>
        [HttpGet("branches-list")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBranchesList([FromQuery] string format = "Excel")
        {
            try
            {
                var request = new ReportRequest(
                    ReportCode: "BRANCHES_LIST",
                    Format: format,
                    Parameters: new Dictionary<string, object>()
                );

                var result = await _reportEngine.GenerateAsync(request, HttpContext.RequestAborted);
                return File(result.Content, result.MimeType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando listado de sucursales");
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpGet("company-legal-sheet")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCompanyLegalSheet([FromQuery] string format = "PdfFicha")
        {
            try
            {
                var allowedFormats = new[] { "PdfFicha", "Pdf", "Excel" };
                if (!allowedFormats.Contains(format, StringComparer.OrdinalIgnoreCase))
                {
                    return BadRequest($"Formato no soportado. Use: {string.Join(", ", allowedFormats)}");
                }

                var request = new ReportRequest(
                    ReportCode: "COMPANY_LEGAL_SHEET",
                    Format: format,
                    Parameters: new Dictionary<string, object>()
                );

                var result = await _reportEngine.GenerateAsync(request, HttpContext.RequestAborted);
                return File(result.Content, result.MimeType, result.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generando ficha legal");
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}
