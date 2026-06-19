using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    /// <summary>
    /// Comando para crear y ejecutar una Acción de Personal Masiva.
    /// </summary>
    public record CreateMassPersonalActionCommand(CreateMassPersonalActionDto Data) : IRequest<MassActionPreviewDto>;
}
