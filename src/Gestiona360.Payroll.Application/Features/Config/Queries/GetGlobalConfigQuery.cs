using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Config.Queries
{
    public record GetGlobalConfigQuery(
        int? YearINSS = null,
        int? YearIR = null,
        int? YearMitrab = null,
        int? YearINATEC = null  
    ) : IRequest<GlobalConfigDto>;
}
