using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.INSSConfigs.Commands
{
    public record CreateNewINSSValidityCommand(
        DateOnly EffectiveFrom,
        string LegalReference,
        decimal RateWorker,
        decimal RateEmployerSmall,
        decimal RateEmployerLarge,
        decimal MaxSalaryCap
    ) : IRequest<Unit>;
}
