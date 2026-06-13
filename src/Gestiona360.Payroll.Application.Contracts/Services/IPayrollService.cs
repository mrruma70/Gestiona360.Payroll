using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;

namespace Gestiona360.Payroll.Application.Contracts.Services
{
    public interface IPayrollService
    {
        Task<ActivePeriodDto?> GetCurrentActivePeriodAsync();
    }
}
