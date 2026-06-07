using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Common;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands
{
    public record ImportDgiTaxBracketCommand(Stream FileStream, string FileName) : IRequest<Result>;

}
