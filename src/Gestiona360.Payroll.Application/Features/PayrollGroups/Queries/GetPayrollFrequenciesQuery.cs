using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using MediatR;

namespace Gestiona360.Payroll.Application.Queries
{
    public record GetPayrollFrequenciesQuery() : IRequest<List<PayrollFrequencyDto>>;
}
