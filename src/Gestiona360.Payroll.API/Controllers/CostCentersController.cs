using Gestiona360.Payroll.Application.Features.CostCenters.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CostCentersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CostCentersController(IMediator mediator) => _mediator = mediator;

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCostCenters()
        {
            var costCenters = await _mediator.Send(new GetActiveCostCentersQuery());
            return Ok(costCenters);
        }
    }
}
