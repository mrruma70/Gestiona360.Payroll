using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Branches
{
    public record DeleteBranchCommand(Guid Id) : IRequest<Unit>;
}
