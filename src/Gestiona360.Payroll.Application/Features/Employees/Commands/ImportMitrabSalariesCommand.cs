using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands
{
    public record ImportMitrabSalariesCommand(Stream FileStream, string FileName, int Strategy) : IRequest<Result>;
}
