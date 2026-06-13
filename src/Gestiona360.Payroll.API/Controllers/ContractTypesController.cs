using Gestiona360.Payroll.Application.Contracts.DTOs;
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
    public class ContractTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context; // ✅ AGREGAR

        // ✅ AGREGAR parámetro del contexto
        public ContractTypesController(IMediator mediator, ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            var types = await _context.ContractTypes
                .Where(ct => ct.IsActive)
                .Select(ct => new ContractTypeDto
                {
                    Id = ct.Id,
                    Name = ct.Name,
                    ApplyINSS = ct.ApplyINSS,
                    ApplyIR = ct.ApplyIR,
                    ApplyINATEC = ct.ApplyINATEC,
                    ApplyVacationsDays = ct.ApplyVacationsDays, // ✅ Verificar que exista en el DTO
                    ApplyThirteenthMonth = ct.ApplyThirteenthMonth,
                    ApplyIndemnity = ct.ApplyIndemnity
                })
                .OrderBy(ct => ct.Name)
                .ToListAsync();
            return Ok(types);
        }
    }
}
