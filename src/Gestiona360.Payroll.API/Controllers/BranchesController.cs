using Gestiona360.Payroll.Application.Contracts.Requests;
using Gestiona360.Payroll.Application.Features.Branches;
using Gestiona360.Payroll.Application.Features.Companies.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BranchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BranchesController(IMediator mediator) => _mediator = mediator;

        /// <summary>
        /// GET: api/branches/all
        /// Obtiene todas las sucursales.
        /// </summary>
        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var branches = await _mediator.Send(new GetAllBranchesQuery());
            return Ok(branches);
        }

        /// <summary>
        /// GET: api/branches/active
        /// Obtiene solo las sucursales activas.
        /// </summary>
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var branches = await _mediator.Send(new GetAllBranchesQuery());
            return Ok(branches.Where(b => b.IsActive));
        }

        /// <summary>
        /// POST: api/branches
        /// Crea una nueva sucursal.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateBranchRequest request)
        {
            try
            {
                var id = await _mediator.Send(new CreateBranchCommand(request));
                return CreatedAtAction(nameof(GetAll), new { id }, new { id, message = "Sucursal creada exitosamente." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al crear la sucursal.", detail = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/branches/{id}
        /// Actualiza una sucursal existente.
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBranchRequest request)
        {
            try
            {
                await _mediator.Send(new UpdateBranchCommand(id, request));
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al actualizar la sucursal.", detail = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/branches/{id}
        /// Desactiva (baja lógica) una sucursal.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _mediator.Send(new DeleteBranchCommand(id));
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error interno al desactivar la sucursal.", detail = ex.Message });
            }
        }
    }
}

