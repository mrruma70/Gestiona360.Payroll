using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BanksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BanksController(ApplicationDbContext context) => _context = context;

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var banks = await _context.Banks
            .Where(b => b.IsActive)
            .Select(b => new BankDto
            {
                Id = b.Id,
                Name = b.Name,
                Code = b.Code,
                AchCode = b.AchCode ?? "",
                IsActive = b.IsActive
            })
            .OrderBy(b => b.Name)
            .ToListAsync();

        return Ok(banks);
    }
}