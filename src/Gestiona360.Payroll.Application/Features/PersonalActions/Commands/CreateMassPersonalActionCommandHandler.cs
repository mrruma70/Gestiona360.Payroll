using System.Text.Json;
using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Contracts.DTOs.PersonalActions;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class CreateMassPersonalActionCommandHandler
        : IRequestHandler<CreateMassPersonalActionCommand, MassActionPreviewDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PersonalActionDomainService _domainService;
        private readonly IMediator _mediator;
        private readonly ILogger<CreateMassPersonalActionCommandHandler> _logger;

        public CreateMassPersonalActionCommandHandler(
            IUnitOfWork unitOfWork,
            PersonalActionDomainService domainService,
            IMediator mediator,
            ILogger<CreateMassPersonalActionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<MassActionPreviewDto> Handle(
            CreateMassPersonalActionCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Data;

            // 1. Obtener Vista Previa y Validar
            var previewResult = await _mediator.Send(
                new GetMassActionPreviewQuery(dto),
                cancellationToken);

            if (previewResult.ValidationErrors.Any() ||
                previewResult.EmployeesPreview.Any(e => !e.IsValid))
            {
                throw new BusinessRuleViolationException(
                    "La acción masiva tiene errores de validación. Revise la vista previa.");
            }

            // 2. Mapear datos al servicio de dominio
            var data = new MassPersonalActionData
            {
                TargetEmployeeIds = dto.TargetEmployeeIds,
                EffectiveDate = dto.EffectiveDate,
                ActionType = dto.ActionType,
                RuleType = dto.RuleType,
                RuleValue = dto.RuleValue,
                CausalDescription = dto.CausalDescription,
                Justification = dto.Justification,
                BatchReference = dto.BatchReference,

                // ✅ CORREGIDO: Serializar cada DocumentAttachmentDto a string JSON
                MasterDocuments = dto.MasterDocuments?
                    .Select(d => JsonSerializer.Serialize(d))
                    .ToList()
            };

            // 3. Crear acciones masivas (lógica de negocio encapsulada)
            var actionsToCreate = await _domainService.CreateMassActionsAsync(data, cancellationToken);

            // 4. Persistir acciones
            await _unitOfWork.PersonalActions.AddRangeAsync(actionsToCreate, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. Ejecutar acciones individualmente (actualizar tablas maestras)
            foreach (var action in actionsToCreate)
            {
                await _mediator.Send(
                    new ExecutePersonalActionCommand(action.Id),
                    cancellationToken);
            }

            _logger.LogInformation(
                "Acción masiva ejecutada exitosamente. Lote: {BatchRef}, Empleados: {Count}",
                dto.BatchReference,
                actionsToCreate.Count);

            // 6. Retornar resumen actualizado
            previewResult.TotalEmployees = actionsToCreate.Count;
            return previewResult;
        }
    }
}
