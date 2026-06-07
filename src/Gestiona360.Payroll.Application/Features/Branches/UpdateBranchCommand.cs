using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.Requests;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Branches
{
    public record UpdateBranchCommand(Guid Id, UpdateBranchRequest Request) : IRequest<Unit>;
}
