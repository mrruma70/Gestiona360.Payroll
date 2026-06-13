using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gestiona360.Payroll.API.Controllers
{
    [ApiController]
    [Route("api/payroll-periods")]
    [Authorize]
    public class PayrollPeriodController : ControllerBase
    {
        private readonly IPayrollService _payrollService;

        public PayrollPeriodController(IPayrollService payrollService)
        {
            _payrollService = payrollService;
        }

        [HttpGet("active")]
        public async Task<ActionResult<ActivePeriodDto>> GetActivePeriod()
        {
            // El controlador solo delega la ejecución
            var period = await _payrollService.GetCurrentActivePeriodAsync();

            if (period == null) return NotFound();

            return Ok(period);
        }
    }
}
