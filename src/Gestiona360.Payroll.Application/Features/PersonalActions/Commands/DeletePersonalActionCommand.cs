using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    /// <summary>
    /// Comando para eliminar una Acción de Personal.
    /// Solo permite eliminar acciones en estado "Pending".
    /// </summary>
    public record DeletePersonalActionCommand(Guid ActionId) : IRequest<Unit>;
}
