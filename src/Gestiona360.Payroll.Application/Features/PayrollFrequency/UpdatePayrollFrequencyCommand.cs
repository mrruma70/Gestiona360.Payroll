using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PayrollFrequency
{
    public record UpdatePayrollFrequencyCommand(
        int Id,
        string Name,
        string Code,
        int DaysPerPeriod,
        int PeriodsPerYear,
        string? Description = null,
        bool? IsActive = null
    ) : IRequest<PayrollFrequencyDto>;
}
