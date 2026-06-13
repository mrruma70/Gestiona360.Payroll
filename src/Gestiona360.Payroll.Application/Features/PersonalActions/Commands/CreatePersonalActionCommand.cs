using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    /// <summary>
    /// Comando para crear una nueva Acción de Personal (Individual o Masiva).
    /// </summary>
    public record CreatePersonalActionCommand(CreatePersonalActionDto Data) : IRequest<Guid>;
}
