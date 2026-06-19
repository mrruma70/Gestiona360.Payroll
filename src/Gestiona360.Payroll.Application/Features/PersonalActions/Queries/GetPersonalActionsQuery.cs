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
    /// Query para obtener el listado paginado de Acciones de Personal.
    /// Filtra obligatoriamente por TargetPayrollPeriodId y soporta búsqueda, filtros y ordenamiento.
    /// </summary>
    public record GetPersonalActionsQuery(PersonalActionFilterDto Filter)
        : IRequest<PersonalActionPagedResultDto>;
}
