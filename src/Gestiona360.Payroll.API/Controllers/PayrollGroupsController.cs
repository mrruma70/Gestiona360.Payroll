using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Features.PayrollGroups.Queries;
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
    public class PayrollGroupsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public PayrollGroupsController(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        // GET: api/payrollgroups
        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<object>>> GetPayrollGroups()
        //{
        //    var groups = await _context.PayrollGroups
        //        .Where(g => g.IsActive)
        //        .Select(g => new { g.Id, g.Name })
        //        .ToListAsync();

        //    return Ok(groups);
        //}

        // GET: api/payrollgroups/{id}/periods
        [HttpGet("{id}/periods")]
        public async Task<ActionResult<IEnumerable<object>>> GetPayrollPeriods(Guid id)
        {
            var periods = await _context.PayrollPeriods
                .Where(p => p.PayrollGroupId == id)
                .OrderByDescending(p => p.StartDate)
                .Select(p => new {
                    p.Id,
                    // ✅ CORRECCIÓN: Generar Name legible
                    Name = $"{p.StartDate:dd/MM/yyyy} - {p.EndDate:dd/MM/yyyy} ({p.Status})",
                    p.StartDate,
                    p.EndDate,
                    p.Status
                })
                .ToListAsync();

            return Ok(periods);
        }

        /// <summary>
        /// Obtiene todos los grupos de nómina activos (para dropdowns).
        /// GET: api/payrollgroups/active
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(List<PayrollGroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive()
        {
            var query = new GetActivePayrollGroupsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Obtiene todos los grupos de nómina.
        /// GET: api/payrollgroups
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<PayrollGroupDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetActivePayrollGroupsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
