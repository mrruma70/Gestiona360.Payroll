using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Services;
using Gestiona360.Payroll.Domain.Shared.Frontend;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class CreatePersonalActionCommandHandler
        : IRequestHandler<CreatePersonalActionCommand, Guid>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PersonalActionDomainService _domainService;
        private readonly IMediator _mediator;
        private readonly ILogger<CreatePersonalActionCommandHandler> _logger;

        public CreatePersonalActionCommandHandler(
            IUnitOfWork unitOfWork,
            PersonalActionDomainService domainService,
            IMediator mediator,
            ILogger<CreatePersonalActionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(
            CreatePersonalActionCommand request,
            CancellationToken cancellationToken)
        {
            var dto = request.Data;

            // 1. Mapear datos al servicio de dominio
            var data = new PersonalActionCreationData
            {
                EmployeeId = dto.EmployeeId,
                EffectiveDate = dto.EffectiveDate,
                ActionType = dto.ActionType,
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

                // ✅ CORREGIDO: Mapear TODAS las propiedades de DocumentAttachmentDto
                Documents = dto.Documents
            };

            // 2. Crear acción (validaciones y lógica de negocio encapsuladas)
            var action = await _domainService.CreatePersonalActionAsync(data, cancellationToken);

            // 3. Persistir (UnitOfWorkBehavior maneja la transacción automáticamente)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 4. Ejecutar inmediatamente si aplica
            if (dto.ExecuteImmediately)
            {
                try
                {
                    var executeCommand = new ExecutePersonalActionCommand(action.Id);
                    await _mediator.Send(executeCommand, cancellationToken);

                    _logger.LogInformation(
                        "Acción de personal creada y ejecutada inmediatamente. Id: {ActionId}",
                        action.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Error al ejecutar inmediatamente la acción de personal. Id: {ActionId}",
                        action.Id);
                    throw new BusinessRuleViolationException(
                        "No se pudo ejecutar la acción inmediatamente. Verifique las reglas de negocio.",
                        ex);
                }
            }
            else
            {
                _logger.LogInformation(
                    "Acción de personal creada como Pendiente. Id: {ActionId}",
                    action.Id);
            }

            return action.Id;
        }
    }
}

