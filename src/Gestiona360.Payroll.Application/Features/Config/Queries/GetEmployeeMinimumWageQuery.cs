using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Config.Queries
{
    public record GetEmployeeMinimumWageQuery(Guid EmployeeId) : IRequest<decimal>;
}
