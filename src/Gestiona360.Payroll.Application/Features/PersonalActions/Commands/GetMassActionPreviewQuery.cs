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
    /// Query para simular una acción masiva y obtener vista previa sin guardar datos.
    /// </summary>
    public record GetMassActionPreviewQuery(CreateMassPersonalActionDto Data) : IRequest<MassActionPreviewDto>;
}
