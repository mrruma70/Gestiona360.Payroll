using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Queries
{
    /// <summary>
    /// Handler que obtiene el detalle completo de una Acción de Personal.
    /// Incluye lógica especial para acciones masivas: carga la lista de empleados afectados del lote.
    /// </summary>

    public class GetPersonalActionDetailQueryHandler
        : IRequestHandler<GetPersonalActionDetailQuery, PersonalActionDetailDto>
    {
        private readonly IPersonalActionRepository _repository;
        private readonly ILogger<GetPersonalActionDetailQueryHandler> _logger;

        public GetPersonalActionDetailQueryHandler(
            IPersonalActionRepository repository,
            ILogger<GetPersonalActionDetailQueryHandler> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PersonalActionDetailDto> Handle(
            GetPersonalActionDetailQuery request,
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Consultando detalle de acción de personal. Id: {ActionId}", request.ActionId);

            // 1. Obtener detalle de la acción
            var action = await _repository.GetDetailByIdAsync(request.ActionId, cancellationToken)
                ?? throw new NotFoundException("Acción de personal", request.ActionId);

            // 2. Parsear documentos adjuntos
            action.DocumentAttachments = PersonalActionMapper.ParseDocumentAttachments(action.DocumentAttachmentsJson);

            // 3. Si es acción masiva, cargar empleados afectados
            if (!string.IsNullOrEmpty(action.BatchReference))
            {
                _logger.LogInformation(
                    "Acción masiva detectada. BatchReference: {BatchRef}. Cargando lista de empleados afectados.",
                    action.BatchReference);

                var batchEmployees = await _repository.GetBatchEmployeesAsync(action.BatchReference, cancellationToken);
                action.AffectedEmployees = batchEmployees;
                action.AffectedCount = batchEmployees.Count;

                _logger.LogInformation(
                    "Cargados {Count} empleados del lote {BatchRef}",
                    batchEmployees.Count, action.BatchReference);
            }

            _logger.LogInformation("Detalle de acción cargado exitosamente. Id: {ActionId}", request.ActionId);

            return action;
        }
    }
}