using Gestiona360.Payroll.Application.Features.INATECConfigs.Commands;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class INATECConfigsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public INATECConfigsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// POST: api/inatecconfigs/new-validity
        /// Crea una nueva vigencia de INATEC, cerrando automáticamente la anterior.
        /// </summary>
        [HttpPost("new-validity")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNewValidity([FromBody] CreateNewINATECValidityRequest request)
        {
            try
            {
                // El Controller convierte el Request en un Command
                var command = new CreateNewINATECValidityCommand(
                    request.EffectiveFrom,
                    request.LegalReference,
                    request.Rate,
                    request.Exceptions
                );

                await _mediator.Send(command);
                return Ok(new { message = "Nueva vigencia de INATEC registrada correctamente." });
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
