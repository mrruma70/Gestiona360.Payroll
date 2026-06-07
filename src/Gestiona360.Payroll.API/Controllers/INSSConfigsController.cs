using Gestiona360.Payroll.Application.Features.INSSConfigs.Commands;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class INSSConfigsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public INSSConfigsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// POST: api/inssconfigs/new-validity
        /// Crea una nueva vigencia de INSS, cerrando automáticamente la anterior.
        /// </summary>
        [HttpPost("new-validity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNewValidity([FromBody] CreateNewINSSValidityRequest request)
        {
            try
            {
                // ✅ El Controller convierte el Request en un Command
                var command = new CreateNewINSSValidityCommand(
                    request.EffectiveFrom,
                    request.LegalReference,
                    request.RateWorker,
                    request.RateEmployerSmall,
                    request.RateEmployerLarge,
                    request.MaxSalaryCap
                );

                await _mediator.Send(command);
                return Ok(new { message = "Nueva vigencia de INSS registrada correctamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno al registrar la vigencia.", detail = ex.Message });
            }
        }
    }
}
