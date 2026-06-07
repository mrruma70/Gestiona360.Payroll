using System.ComponentModel.DataAnnotations;
using Gestiona360.Payroll.API.Contracts.Common;
using Gestiona360.Payroll.Application.Commands;
using Gestiona360.Payroll.Application.Commands.PayrollFrequency;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PayrollFrequenciesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PayrollFrequenciesController(IMediator mediator) => _mediator = mediator;

        [HttpGet("all")] 
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var result = await _mediator.Send(new GetPayrollFrequenciesQuery());
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<PayrollFrequencyDto>>> GetPaged(
     int page = 0,
     int pageSize = 10,
     string? sort = null,
     string? dir = null)
        {
            var result = await _mediator.Send(
                new GetPayrollFrequenciesPagedQuery(page, pageSize, sort, dir)
            );

            return Ok(result);
        }

        [HttpPost]
        [ProducesResponseType(typeof(PayrollFrequencyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create(CreatePayrollFrequencyCommand request)
        {

            try
            {
                var result = await _mediator.Send(request);
                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return BadRequest(new { title = "Errores de validación", errors });
            }

            

        }

        [HttpPut]
        public async Task<IActionResult> Update(UpdatePayrollFrequencyCommand command)
        {
            try
            {
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (FluentValidation.ValidationException ex)
            {
                var errors = ex.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray());
                return BadRequest(new { title = "Errores de validación", errors });
            }
        }

        // ✅ Mejora Delete para manejar 404
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mediator.Send(new DeletePayrollFrequencyCommand(id));
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Frecuencia con ID {id} no encontrada" });
            }
        }
    }
}
