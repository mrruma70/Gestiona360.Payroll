// src/Gestiona360.Payroll.API/Controllers/EmployeesController.cs

using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Contracts.Reports;
using Gestiona360.Payroll.Application.Contracts.Requests;
using Gestiona360.Payroll.Application.Features.Config.Queries;
using Gestiona360.Payroll.Application.Features.Employees.Commands;
using Gestiona360.Payroll.Application.Features.Employees.Queries;
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmployeesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _env;
        private readonly IReportEngine _reportEngine;
        private readonly ILogger<EmployeesController> _logger;
        private readonly IEmployeeExportService _exportService;
        private readonly EmployeeBarcodeService _barcodeService;

        public EmployeesController(
            IMediator mediator,
            IWebHostEnvironment env,
            IReportEngine reportEngine,
            ILogger<EmployeesController> logger,
            IEmployeeExportService exportService,
            EmployeeBarcodeService barcodeService)
        {
            _mediator = mediator;
            _env = env;
            _logger = logger;
            _reportEngine = reportEngine;
            _exportService = exportService;
            _barcodeService = barcodeService;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Planillero,RRHH")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeRequest request)
        {
            try
            {
                var command = new CreateEmployeeCommand(request);
                var employeeId = await _mediator.Send(command);
                return Ok(new { id = employeeId, message = "Empleado creado exitosamente" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al crear empleado", detail = ex.Message });
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(EmployeeSearchResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(
            [FromQuery] string? search = null,
            [FromQuery] Guid? branchId = null,
            [FromQuery] int? contractTypeId = null,
            [FromQuery] string? status = null,
            [FromQuery] Guid? jobPositionId = null,
            [FromQuery] int? employmentStatus = null,
            [FromQuery] bool? isTrustEmployee = null,
            [FromQuery] bool? isForeignWorker = null,
            [FromQuery] bool? isOnProbation = null,
            [FromQuery] bool? isRehire = null,
            [FromQuery] Guid? payrollGroupId = null)
        {
            var query = new GetEmployeesQuery(
                search, branchId, contractTypeId, status, jobPositionId,
                employmentStatus, isTrustEmployee, isForeignWorker,
                isOnProbation, isRehire, payrollGroupId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var query = new GetEmployeesQuery(Status: "active");
            var result = await _mediator.Send(query);
            return Ok(result.Employees);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(Guid id)
        {
            try
            {
                var query = new GetEmployeeDetailQuery(id);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpGet("{id:guid}/edit")] // ✅ Cambiado para evitar conflicto con {id}
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var query = new GetEmployeeByIdQuery(id);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        [HttpPut("{id:guid}")]
        [Authorize(Roles = "Admin,Planillero,RRHH")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmployeeRequest request)
        {
            try
            {
                if (id != request.Id)
                    return BadRequest(new { error = "El ID de la URL no coincide con el ID del cuerpo." });

                var command = new UpdateEmployeeCommand(request);
                await _mediator.Send(command);
                return Ok(new { message = "Empleado actualizado exitosamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al actualizar empleado.", detail = ex.Message });
            }
        }

        [HttpPut("{id}/toggle-active")]
        public async Task<IActionResult> ToggleActive(Guid id)
        {
            try
            {
                var command = new ToggleEmployeeStatusCommand(id);
                await _mediator.Send(command);
                return Ok(new { message = "Estado del empleado actualizado correctamente." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al cambiar el estado.", detail = ex.Message });
            }
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> GeneratePdf(Guid id)
        {
            try
            {
                var webRootPath = _env.WebRootPath;
                if (string.IsNullOrEmpty(webRootPath))
                {
                    webRootPath = Path.Combine(_env.ContentRootPath, "wwwroot");
                }

                var query = new GenerateEmployeePdfQuery(id, webRootPath);
                var pdfBytes = await _mediator.Send(query);
                return File(pdfBytes, "application/pdf", $"Ficha_Empleado_{id}.pdf");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al generar el PDF", detail = ex.Message });
            }
        }

        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel(
            [FromQuery] string? search = null,
            [FromQuery] Guid? branchId = null,
            [FromQuery] int? contractTypeId = null,
            [FromQuery] int? employmentStatus = null,
            [FromQuery] Guid? jobPositionId = null,
            [FromQuery] bool? isTrustEmployee = null,
            [FromQuery] bool? isForeignWorker = null,
            [FromQuery] bool? isOnProbation = null,
            [FromQuery] bool? isRehire = null,
            [FromQuery] Guid? payrollGroupId = null)
        {
            var filters = new EmployeeExportFilters
            {
                Search = search,
                BranchId = branchId,
                ContractTypeId = contractTypeId,
                EmploymentStatus = employmentStatus,
                JobPositionId = jobPositionId,
                IsTrustEmployee = isTrustEmployee,
                IsForeignWorker = isForeignWorker,
                IsOnProbation = isOnProbation,
                IsRehire = isRehire,
                PayrollGroupId = payrollGroupId
            };

            var excelBytes = await _exportService.ExportToExcelAsync(filters);
            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Empleados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        [HttpGet("{id:guid}/personal-actions")]
        [ProducesResponseType(typeof(List<PersonalActionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPersonalActions(Guid id)
        {
            var query = new GetEmployeePersonalActionsQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}/payroll-history")]
        [ProducesResponseType(typeof(EmployeePayrollHistoryResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayrollHistory(Guid id)
        {
            var query = new GetEmployeePayrollHistoryQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}/concepts")]
        [ProducesResponseType(typeof(EmployeeConceptsResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConcepts(Guid id)
        {
            var query = new GetEmployeeConceptsQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id:guid}/shifts")]
        [ProducesResponseType(typeof(EmployeeShiftsResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetShifts(Guid id)
        {
            var query = new GetEmployeeShiftsQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var query = new GetActiveEmployeesQuery();
            var employees = await _mediator.Send(query);
            return Ok(employees);
        }

        // ✅ REFACTORIZADO: Usa MediatR en lugar de _context
        [HttpGet("{id}/minimum-wage")]
        public async Task<ActionResult<decimal>> GetMinimumWage(Guid id)
        {
            var query = new GetEmployeeMinimumWageQuery(id);
            var minWage = await _mediator.Send(query);
            return Ok(minWage);
        }

        // ✅ REFACTORIZADO: Usa MediatR en lugar de _context
        [HttpGet("check-rehire/{identification}")]
        [ProducesResponseType(typeof(RehireCheckResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckRehire(string identification)
        {
            var query = new CheckEmployeeRehireQuery(identification);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // ✅ REFACTORIZADO: Usa MediatR en lugar de _context
        [HttpGet("{id:guid}/qrcode")]
        [AllowAnonymous] // ✅ Permite acceso sin token
        public async Task<IActionResult> GetEmployeeQrCode(Guid id)
        {
            var query = new GetEmployeeBarcodeInfoQuery(id);
            var info = await _mediator.Send(query);

            var payload = string.IsNullOrEmpty(info.ExistingBarcode)
                ? _barcodeService.GeneratePayload(info.Code, info.CompanyId)
                : info.ExistingBarcode;

            var qrBytes = _barcodeService.GenerateQrCode(payload, pixelsPerModule: 12);
            return File(qrBytes, "image/png");
        }

        // ✅ REFACTORIZADO: Usa MediatR en lugar de _context
        [HttpGet("{id:guid}/credential-pdf")]
        public async Task<IActionResult> GetEmployeeCredentialPdf(Guid id)
        {
            try
            {
                var query = new GenerateEmployeeCredentialQuery(id, _env.WebRootPath);
                var pdfBytes = await _mediator.Send(query);

                var barcodeQuery = new GetEmployeeBarcodeInfoQuery(id);
                var info = await _mediator.Send(barcodeQuery);
                var fileName = $"Credencial_{info.Code}.pdf";

                return File(pdfBytes, "application/pdf", fileName);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error generando credencial: {ex.Message}" });
            }
        }

        // ✅ REFACTORIZADO: Usa MediatR en lugar de _context
        [HttpGet("{id:guid}/barcode-payload")]
        public async Task<IActionResult> GetEmployeeBarcodePayload(Guid id)
        {
            var query = new GetEmployeeBarcodeInfoQuery(id);
            var info = await _mediator.Send(query);

            var payload = string.IsNullOrEmpty(info.ExistingBarcode)
                ? _barcodeService.GeneratePayload(info.Code, info.CompanyId)
                : info.ExistingBarcode;

            return Ok(new { payload });
        }
    }
}