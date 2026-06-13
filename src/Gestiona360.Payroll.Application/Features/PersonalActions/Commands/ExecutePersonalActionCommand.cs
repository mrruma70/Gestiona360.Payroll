using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    /// <summary>
    /// Comando para cambiar el estado de una acción a "Executed" y aplicar los cambios a las tablas maestras.
    /// Soporta ejecución individual o masiva (por BatchReference).
    /// </summary>
    public record ExecutePersonalActionCommand(Guid ActionId) : IRequest<Unit>;
}
