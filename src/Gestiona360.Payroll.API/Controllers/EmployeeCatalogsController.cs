using Gestiona360.Payroll.Application.Features.Employees.Commands;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeeCatalogsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public EmployeeCatalogsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// POST: api/employees/dgi-tax-brackets/import
        /// Reemplaza completamente la matriz progresiva del IR anual (Ley 822 de Nicaragua).
        /// </summary>
        [HttpPost("dgi-tax-brackets/import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportDgiTaxBracket(IFormFile file, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se proporcionó un archivo CSV válido o el archivo está vacío.");
            }

            // ✅ Cambiar de .xml a .csv
            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("El archivo debe tener una extensión .csv válida.");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var command = new ImportDgiTaxBracketCommand(stream, file.FileName);
                var result = await _mediator.Send(command, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.Error);
                }

                return Ok(new { Message = "Matriz progresiva DGI IR procesada y reemplazada exitosamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error crítico del sistema: {ex.Message}");
            }
        }

        /// <summary>
        /// POST: api/employees/mitrab-salaries/import
        /// Ejecuta la carga de salarios mínimos sectoriales según acuerdo del MITRAB.
        /// </summary>
        [HttpPost("mitrab-salaries/import")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ImportMitrabSalaries([FromForm] IFormFile file, [FromForm] int strategy, CancellationToken cancellationToken)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No se proporcionó un archivo CSV válido o el archivo está vacío.");
            }

            if (!Path.GetExtension(file.FileName).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("El archivo debe tener una extensión .csv válida.");
            }

            if (strategy != 0 && strategy != 1)
            {
                return BadRequest("Estrategia de importación no válida. Use 0 para actualizar/fusionar o 1 para limpiar y reimportar.");
            }

            try
            {
                using var stream = file.OpenReadStream();
                var command = new ImportMitrabSalariesCommand(stream, file.FileName, strategy);
                var result = await _mediator.Send(command, cancellationToken);

                if (!result.IsSuccess)
                {
                    return BadRequest(result.Error);
                }

                return Ok(new { Message = "Acuerdo de salarios mínimos MITRAB ejecutado y procesado con éxito." });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error crítico del sistema: {ex.Message}");
            }
        }
    }
}