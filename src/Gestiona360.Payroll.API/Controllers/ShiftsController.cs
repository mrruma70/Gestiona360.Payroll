using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShiftsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public ShiftsController(ApplicationDbContext context) => _context = context;

        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            var shifts = await _context.Shifts
                .Where(s => s.IsActive)
                .Select(s => new { s.Id, s.Name })
                .ToListAsync();
            return Ok(shifts);
        }
    }
}
