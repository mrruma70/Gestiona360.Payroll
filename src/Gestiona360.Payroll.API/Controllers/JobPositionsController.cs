using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPositionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMediator _mediator;

        public JobPositionsController(ApplicationDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var positions = await _context.JobPositions
                .Include(jp => jp.OccupationalRisk)
                .Include(jp => jp.MinimumWage)
                .Where(jp => jp.MinimumWage != null)
                .Select(jp => new JobPositionDto
                {
                    Id = jp.Id,
                    Code = jp.Code,
                    Name = jp.Name,
                    Category = jp.Category,
                    OccupationalRiskName = jp.OccupationalRisk != null ? jp.OccupationalRisk.Name : "",
                    MinimumWageAmount = jp.MinimumWage != null ? jp.MinimumWage.MonthlyAmountNIO : 0
                })
                .OrderBy(jp => jp.Name)
                .ToListAsync();

            return Ok(positions);
        }
    }
}
