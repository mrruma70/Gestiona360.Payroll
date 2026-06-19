using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Queries
{
    /// <summary>
    /// Query para obtener el detalle completo de una Acción de Personal.
    /// Si la acción es masiva (BatchReference != null), incluye la lista de empleados afectados.
    /// </summary>
    public record GetPersonalActionDetailQuery(Guid ActionId) : IRequest<PersonalActionDetailDto>;
}
