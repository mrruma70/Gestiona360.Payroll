using Gestiona360.Payroll.Application.Contracts.Requests;
using Gestiona360.Payroll.Application.Features.Companies.Commands;
using Gestiona360.Payroll.Application.Features.Companies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CompaniesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("main")]
        public async Task<IActionResult> GetMain()
        {
            var company = await _mediator.Send(new GetMainCompanyQuery());
            if (company == null) return NotFound();
            return Ok(company);
        }

        [HttpPut("main")]
        public async Task<IActionResult> UpdateMain([FromBody] UpdateCompanyRequest request)
        {
            try
            {
                await _mediator.Send(new UpdateCompanyCommand(request));
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
