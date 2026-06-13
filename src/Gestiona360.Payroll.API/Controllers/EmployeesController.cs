using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Contracts.Reports;
using Gestiona360.Payroll.Application.Contracts.Requests;
using Gestiona360.Payroll.Application.Features.Employees.Commands;
using Gestiona360.Payroll.Application.Features.Employees.Exports;
using Gestiona360.Payroll.Application.Features.Employees.Queries;
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        private readonly EmployeeExportService _exportService;
        private readonly ApplicationDbContext _context;
        private readonly EmployeeBarcodeService _barcodeService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EmployeesController(
            IMediator mediator,
            IWebHostEnvironment env,
            IReportEngine reportEngine,
            ILogger<EmployeesController> logger,
            EmployeeExportService exportService,
            ApplicationDbContext dbContext, EmployeeBarcodeService barcodeService, IWebHostEnvironment webHostEnvironment)   
        {
            _mediator = mediator;
            _env = env;
            _logger = logger;
            _reportEngine = reportEngine;
            _exportService = exportService;
            _context = dbContext;
            _barcodeService = barcodeService;
            _webHostEnvironment = webHostEnvironment;
          
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
                search,
                branchId,
                contractTypeId,
                status,
                jobPositionId,
                employmentStatus,
                isTrustEmployee,
                isForeignWorker,
                isOnProbation,
                isRehire,
                payrollGroupId 
            );
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// GET: api/employees/active
        /// Obtiene solo empleados activos (para dropdowns).
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            // ✅ Usar el record con parámetros nombrados
            var query = new GetEmployeesQuery(Status: "active");
            var result = await _mediator.Send(query);
            return Ok(result.Employees);
        }

        /// <summary>
        /// GET: api/employees/{id}
        /// Obtiene el detalle completo de un empleado.
        /// </summary>
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

        /// <summary>
        /// GET: api/employees/{id}
        /// Obtiene el detalle completo de un empleado para edición.
        /// </summary>
        [HttpGet("{id:guid}")]
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

        /// <summary>
        /// PUT: api/employees/{id}
        /// Actualiza los datos de un empleado existente.
        /// </summary>
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

        /// <summary>
        /// PUT: api/employees/{id}/toggle-active
        /// Cambia el estado (Activo/Inactivo) de un empleado. 
        /// No aplica es ilegal esto************** para eliminar empleados, solo para inactivarlos.
        /// </summary>
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




        /// <summary>
        /// GET: api/employees/{id}/pdf
        /// Genera y descarga la ficha del empleado en PDF.
        /// </summary>
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

                // ✅ Pasa la ruta al Query
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

        /// <summary>
        /// GET: api/employees/export/excel
        /// Exporta empleados a Excel usando el motor de reportes.
        /// </summary>
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
     [FromQuery] Guid? payrollGroupId = null)  // ✅ NUEVO
        {
            var excelBytes = await _exportService.ExportToExcelAsync(
                search, branchId, contractTypeId, employmentStatus,
                jobPositionId, isTrustEmployee, isForeignWorker,
                isOnProbation, isRehire, payrollGroupId);  // ✅ NUEVO

            return File(excelBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Empleados_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        /// <summary>
        /// Obtiene el historial de acciones de personal de un empleado específico.
        /// </summary>
        [HttpGet("{id:guid}/personal-actions")]
        [ProducesResponseType(typeof(List<PersonalActionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPersonalActions(Guid id)
        {
            var query = new GetEmployeePersonalActionsQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        /// <summary>
        /// Obtiene el historial de nóminas de un empleado específico.
        /// </summary>
        [HttpGet("{id:guid}/payroll-history")]
        [ProducesResponseType(typeof(EmployeePayrollHistoryResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPayrollHistory(Guid id)
        {
            var query = new GetEmployeePayrollHistoryQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        /// <summary>
        /// Obtiene los conceptos recurrentes y retenciones judiciales de un empleado específico (Solo Consulta).
        /// </summary>
        [HttpGet("{id:guid}/concepts")]
        [ProducesResponseType(typeof(EmployeeConceptsResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConcepts(Guid id)
        {
            var query = new GetEmployeeConceptsQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene el turno actual y el historial de turnos de un empleado (Solo Consulta).
        /// </summary>
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
            var employees = await _context.Employees
                .Where(e => e.IsActive)
                .Select(e => new { e.Id, e.Code, e.FirstName, e.LastName, e.JobGrade.Name, e.BaseSalary })
                .ToListAsync();
            return Ok(employees);
        }

        [HttpGet("{id}/minimum-wage")]
        public async Task<ActionResult<decimal>> GetMinimumWage(Guid id)
        {
            var employee = await _context.Employees
                .Include(e => e.JobGrade)
                    .ThenInclude(jg => jg.JobPosition)
                        .ThenInclude(jp => jp.MinimumWage)
                .FirstOrDefaultAsync(e => e.Id == id);
            if (employee == null) return NotFound();
            var minWage = employee.JobGrade?.JobPosition?.MinimumWage?.MonthlyAmountNIO ?? 0;
            return Ok(minWage);
        }

        /// <summary>
        /// Verifica si una cédula corresponde a un empleado terminado (reingreso).
        /// </summary>
        [HttpGet("check-rehire/{identification}")]
        [ProducesResponseType(typeof(RehireCheckResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckRehire(string identification)
        {
            var existingEmployee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Identification == identification);

            if (existingEmployee == null)
            {
                return Ok(new RehireCheckResponse { IsRehire = false });
            }

            if (existingEmployee.EmploymentStatus == EmploymentStatus.Terminated)
            {
                return Ok(new RehireCheckResponse
                {
                    IsRehire = true,
                    PreviousEmployeeName = $"{existingEmployee.FirstName} {existingEmployee.LastName}",
                    PreviousEmployeeCode = existingEmployee.Code,
                    TerminationDate = existingEmployee.TerminationDate
                });
            }

            return Ok(new RehireCheckResponse
            {
                IsRehire = false,
                ErrorMessage = $"Ya existe un empleado activo con la cédula {identification}."
            });
        }

        /// <summary>
        /// Obtiene el código QR del empleado en formato PNG.
        /// </summary>
        [HttpGet("{id:guid}/qrcode")]
        public async Task<IActionResult> GetEmployeeQrCode(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound("Empleado no encontrado");

            var payload = string.IsNullOrEmpty(employee.CodigoBarra)
                ? _barcodeService.GeneratePayload(employee.Code, employee.CompanyId)
                : employee.CodigoBarra;

            var qrBytes = _barcodeService.GenerateQrCode(payload, pixelsPerModule: 12);
            return File(qrBytes, "image/png");
        }

        /// <summary>
        /// Genera el PDF de credencial tipo carnet del empleado.
        /// </summary>
        [HttpGet("{id:guid}/credential-pdf")]
        public async Task<IActionResult> GetEmployeeCredentialPdf(Guid id)
        {
            try
            {
                var query = new Gestiona360.Payroll.Application.Features.Employees.Queries.GenerateEmployeeCredentialQuery(
                    id,
                    _webHostEnvironment.WebRootPath
                );

                var pdfBytes = await _mediator.Send(query);

                var employee = await _context.Employees.FindAsync(id);
                var fileName = $"Credencial_{employee?.Code ?? id.ToString()}.pdf";

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

        /// <summary>
        /// Obtiene el payload (texto) para generar el código de barras en el frontend.
        /// </summary>
        [HttpGet("{id:guid}/barcode-payload")]
        public async Task<IActionResult> GetEmployeeBarcodePayload(Guid id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
                return NotFound("Empleado no encontrado");

            var payload = string.IsNullOrEmpty(employee.CodigoBarra)
                ? _barcodeService.GeneratePayload(employee.Code, employee.CompanyId)
                : employee.CodigoBarra;

            return Ok(new { payload });
        }

        private class RehireCheckResponse
        {
            public bool IsRehire { get; set; }
            public string? PreviousEmployeeName { get; set; }
            public string? PreviousEmployeeCode { get; set; }
            public DateTime? TerminationDate { get; set; }
            public string? ErrorMessage { get; set; }
        }
    }


}
