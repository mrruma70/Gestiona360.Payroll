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
    public class MunicipalitiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MunicipalitiesController(ApplicationDbContext context) => _context = context;

        [HttpGet("by-department/{departmentId:int}")]
        public async Task<IActionResult> GetByDepartment(int departmentId)
        {
            var municipalities = await _context.Municipalities
                .Where(m => m.DepartmentId == departmentId && m.IsActive)
                .OrderBy(m => m.Name)
                .Select(m => new { m.Id, m.Name, m.DepartmentId })
                .ToListAsync();
            return Ok(municipalities);
        }
    }
}
