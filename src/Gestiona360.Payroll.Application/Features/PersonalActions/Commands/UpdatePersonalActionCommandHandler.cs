using System;
using System.Collections.Generic;
using System.Text;
using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using Gestiona360.Payroll.Domain.Shared.Frontend.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class UpdatePersonalActionCommandHandler
            : IRequestHandler<UpdatePersonalActionCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PersonalActionDomainService _domainService;
        private readonly IMediator _mediator;
        private readonly ILogger<UpdatePersonalActionCommandHandler> _logger;

        public UpdatePersonalActionCommandHandler(
            IUnitOfWork unitOfWork,
            PersonalActionDomainService domainService,
            IMediator mediator,
            ILogger<UpdatePersonalActionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(
            UpdatePersonalActionCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Data;

            // 1. Buscar la acción existente
            var existingAction = await _unitOfWork.PersonalActions
                .GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException($"Acción de personal con Id {request.Id} no encontrada.");

            // 2. Validar que esté en estado Borrador/Draft
            if (existingAction.Status != ActionStatus.Pending)
            {
                throw new BusinessRuleViolationException(
                    $"Solo se pueden editar acciones en estado 'Borrador'. Estado actual: {existingAction.Status}");
            }

            //// 3. Validar que el empleado no haya cambiado (no se permite cambiar el empleado)
            //if (existingAction.EmployeeId != dto.EmployeeId)
            //{
            //    throw new BusinessRuleViolationException(
            //        "No se permite cambiar el empleado asociado a una acción de personal.");
            //}

            // 4. Validar que el tipo de acción no haya cambiado
            if (!Enum.TryParse<ActionType>(dto.ActionType, true, out var dtoActionType) ||
       existingAction.ActionType != dtoActionType)
            {
                throw new BusinessRuleViolationException(
                    "No se permite cambiar el tipo de acción de personal.");
            }

            // 5. Mapear datos actualizados al servicio de dominio
            var data = new PersonalActionUpdateData
            {
                ActionId = request.Id,
                EffectiveDate = dto.EffectiveDate,
                CausalDescription = dto.CausalDescription,
                Justification = dto.Justification,
                ExecuteImmediately = dto.ExecuteImmediately,
                NewBaseSalary = dto.NewBaseSalary,
                NewJobGradeId = dto.NewJobGradeId,
                NewContractTypeId = dto.NewContractTypeId,
                NewCostCenterId = dto.NewCostCenterId,
                NewShiftId = dto.NewShiftId,
                NewBankId = dto.NewBankId,
                NewBankAccountNumber = dto.NewBankAccountNumber,
                NewHealthProviderId = dto.NewHealthProviderId,
                Documents = dto.Documents
            };

            // 6. Actualizar la acción (validaciones de negocio encapsuladas)
            var updatedAction = await _domainService.UpdatePersonalActionAsync(data, existingAction, cancellationToken);

            // 7. Persistir cambios
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 8. Ejecutar inmediatamente si aplica
            if (dto.ExecuteImmediately)
            {
                try
                {
                    var executeCommand = new ExecutePersonalActionCommand(updatedAction.Id);
                    await _mediator.Send(executeCommand, cancellationToken);

                    _logger.LogInformation(
                        "Acción de personal actualizada y ejecutada inmediatamente. Id: {ActionId}",
                        updatedAction.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error al ejecutar inmediatamente la acción de personal actualizada. Id: {ActionId}",
                        updatedAction.Id);
                    throw new BusinessRuleViolationException(
                        "No se pudo ejecutar la acción inmediatamente. Verifique las reglas de negocio.",
                        ex);
                }
            }
            else
            {
                _logger.LogInformation(
                    "Acción de personal actualizada como Borrador. Id: {ActionId}",
                    updatedAction.Id);
            }

            return updatedAction.Id;
        }
    }
}
