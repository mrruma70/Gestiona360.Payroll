using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Application.Features.PersonalActions.Commands;
using Gestiona360.Payroll.Application.Features.PersonalActions.Queries;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers;

/// <summary>
/// Controlador para consultas de Acciones de Personal.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonalActionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonalActionsController(IMediator mediator)
    {
        _mediator = mediator;
    }



    /// <summary>
    /// Obtiene el listado paginado de Acciones de Personal filtrado por período de nómina.
    /// </summary>
    /// <param name="filter">Filtros de búsqueda, paginación y ordenamiento</param>
    /// <returns>Resultado paginado con las acciones encontradas</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PersonalActionPagedResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPersonalActions(
    [FromQuery] Guid PayrollPeriodId,
    [FromQuery] int PageNumber = 1,
    [FromQuery] int PageSize = 10,
    [FromQuery] Guid? PayrollGroupId = null,
    [FromQuery] string? SearchTerm = null,
    [FromQuery] string? Status = null)
    {
        // Tu lógica existente...
        var filter = new PersonalActionFilterDto
        {
            PayrollPeriodId = PayrollPeriodId,
            PageNumber = PageNumber,
            PageSize = PageSize,
            PayrollGroupId = PayrollGroupId,
            SearchTerm = SearchTerm,
            Status = Status
        };

        var query = new GetPersonalActionsQuery(filter);
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene el detalle completo de una Acción de Personal específica.
    /// Si la acción es masiva, incluye la lista de empleados afectados.
    /// </summary>
    /// <param name="id">ID de la acción de personal</param>
    /// <returns>Detalle completo de la acción</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PersonalActionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPersonalActionDetail(Guid id)
    {
        try
        {
            var query = new GetPersonalActionDetailQuery(id);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Crea una nueva Acción de Personal (Individual o Masiva).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreatePersonalAction([FromBody] CreatePersonalActionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var command = new CreatePersonalActionCommand(dto);
        var actionId = await _mediator.Send(command);

        return CreatedAtAction(nameof(GetPersonalActionDetail), new { id = actionId }, actionId);
    }

    /// <summary>
    /// Actualiza una Acción de Personal existente (solo en estado Borrador).
    /// </summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdatePersonalAction(Guid id, [FromBody] UpdatePersonalActionDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (id != dto.Id)
            return BadRequest("El ID de la ruta no coincide con el ID del cuerpo.");

        var command = new UpdatePersonalActionCommand(id, dto);
        var actionId = await _mediator.Send(command);

        return NoContent();
    }

    /// <summary>
    /// Obtiene el detalle de una Acción de Personal por ID.
    /// </summary>
    //[HttpGet("{id:guid}")]
    //[ProducesResponseType(typeof(PersonalActionDetailDto), StatusCodes.Status200OK)]
    //[ProducesResponseType(StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> GetPersonalActionDetail(Guid id)
    //{
    //    var query = new GetPersonalActionDetailQuery(id);
    //    var result = await _mediator.Send(query);

    //    if (result == null)
    //        return NotFound();

    //    return Ok(result);
    //}

    /// <summary>
    /// Ejecuta una Acción de Personal, aplicando los cambios a las tablas maestras.
    /// </summary>
    [HttpPost("{id:guid}/execute")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExecutePersonalAction(Guid id)
    {
        try
        {
            var command = new ExecutePersonalActionCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    /// <summary>
    /// Elimina una Acción de Personal (Solo si está en estado Pending).
    /// </summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeletePersonalAction(Guid id)
    {
        try
        {
            var command = new DeletePersonalActionCommand(id);
            await _mediator.Send(command);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}