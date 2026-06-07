using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class JobGradesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public JobGradesController(ApplicationDbContext context) => _context = context;

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var grades = await _context.JobGrades
            .Include(jg => jg.JobPosition)
                .ThenInclude(jp => jp.MinimumWage)
            .Include(jg => jg.JobPosition)
                .ThenInclude(jp => jp.OccupationalRisk)
            .Where(jg => jg.IsActive)
            .Select(jg => new JobGradeDto
            {
                Id = jg.Id,
                Code = jg.Code,
                Name = jg.Name,
                JobPositionName = jg.JobPosition.Name,
                JobPositionId = jg.JobPositionId,
                BaseSalaryMultiplier = jg.BaseSalaryMultiplier,
                MinWageNIO = jg.JobPosition.MinimumWage != null ? jg.JobPosition.MinimumWage.MonthlyAmountNIO : 0,
                OccupationalRiskName = jg.JobPosition.OccupationalRisk != null ? jg.JobPosition.OccupationalRisk.Name : "",
                IsActive = jg.IsActive
            })
            .OrderBy(jg => jg.JobPositionName)
            .ThenBy(jg => jg.Name)
            .ToListAsync();

        return Ok(grades);
    }
}