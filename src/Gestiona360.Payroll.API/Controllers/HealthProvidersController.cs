using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HealthProvidersController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HealthProvidersController(ApplicationDbContext context) => _context = context;

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var providers = await _context.HealthProviders
            .Where(hp => hp.IsActive)
            .Select(hp => new HealthProviderDto
            {
                Id = hp.Id,
                Name = hp.Name,
                Type = hp.Type,
                ContactPhone = hp.ContactPhone,
                IsActive = hp.IsActive
            })
            .OrderBy(hp => hp.Name)
            .ToListAsync();

        return Ok(providers);
    }
}