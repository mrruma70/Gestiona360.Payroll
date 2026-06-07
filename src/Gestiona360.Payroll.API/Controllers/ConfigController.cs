using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Features.Config.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConfigController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("global")]
        public async Task<IActionResult> GetGlobalConfig(
         [FromQuery] int? yearINSS = null,
         [FromQuery] int? yearIR = null,
         [FromQuery] int? yearMitrab = null,
         [FromQuery] int? yearINATEC = null)
        {
            try
            {
                var query = new GetGlobalConfigQuery(yearINSS, yearIR, yearMitrab, yearINATEC);
                var config = await _mediator.Send(query);
                return Ok(config);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al cargar configuración global", error = ex.Message });
            }
        }
    }
}
