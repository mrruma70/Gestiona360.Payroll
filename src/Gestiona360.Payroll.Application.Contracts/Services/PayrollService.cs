using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Gestiona360.Payroll.Application.Contracts.Services
{
    public class PayrollPeriodService : IPayrollService
    {
        private readonly IApplicationDbContext _context;

        public PayrollPeriodService(IApplicationDbContext context) => _context = context;

        public async Task<ActivePeriodDto?> GetCurrentActivePeriodAsync()
        {
            // Filtramos por Status 'Open'. 
            // NOTA: Si tu sistema usa IsActive para habilitar, asegúrate de que esté en 1
            var period = await _context.PayrollPeriods
                .OrderByDescending(p => p.StartDate)
                .FirstOrDefaultAsync(p => p.Status == "Open" && p.IsDeleted == false);

            if (period == null) return null;

            return new ActivePeriodDto
            {
                Name = period.StartDate.ToString("MMMM yyyy"),
                Sequence = period.PeriodNumber,
                Status = period.Status
            };
        }
    }
}
