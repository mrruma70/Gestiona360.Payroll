using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using Gestiona360.Payroll.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Queries
{
    /// <summary>
    /// Handler que ejecuta la consulta paginada de Acciones de Personal.
    /// Implementa LEFT JOINs con todas las entidades relacionadas para obtener nombres legibles.
    /// </summary>
    public class GetPersonalActionsQueryHandler
       : IRequestHandler<GetPersonalActionsQuery, PersonalActionPagedResultDto>
    {
        private readonly IPersonalActionRepository _repository;
        private readonly ILogger<GetPersonalActionsQueryHandler> _logger;

        public GetPersonalActionsQueryHandler(
            IPersonalActionRepository repository,
            ILogger<GetPersonalActionsQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PersonalActionPagedResultDto> Handle(
            GetPersonalActionsQuery request,
            CancellationToken cancellationToken)
        {
            var filter = request.Filter;

            _logger.LogInformation(
                "Consultando acciones de personal. Período: {PeriodId}, Página: {Page}, Tamaño: {Size}",
                filter.PayrollPeriodId, filter.PageNumber, filter.PageSize);

            // Ejecutar consulta paginada con filtros
            var result = await _repository.GetPagedAsync(filter, cancellationToken);

            _logger.LogInformation(
                "Consulta completada. Total: {Total}, Página: {Page}, Registros: {Count}",
                result.TotalCount, filter.PageNumber, result.Items.Count);

            return result;
        }
    }
}
