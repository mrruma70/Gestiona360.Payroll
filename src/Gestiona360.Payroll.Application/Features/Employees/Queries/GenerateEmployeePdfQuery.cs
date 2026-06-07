using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries
{
    public record GenerateEmployeePdfQuery(Guid EmployeeId, string WebRootPath) : IRequest<byte[]>;
}
