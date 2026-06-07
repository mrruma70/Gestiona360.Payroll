using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.INATECConfigs.Commands
{
    public record CreateNewINATECValidityCommand(
        DateOnly EffectiveFrom,
        string LegalReference,
        decimal Rate,
        string Exceptions
    ) : IRequest<Unit>;
}
