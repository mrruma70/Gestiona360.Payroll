using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands
{
    public class DeletePersonalActionCommandHandler : IRequestHandler<DeletePersonalActionCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PersonalActionDomainService _domainService;
        private readonly ILogger<DeletePersonalActionCommandHandler> _logger;

        public DeletePersonalActionCommandHandler(
            IUnitOfWork unitOfWork,
            PersonalActionDomainService domainService,
            ILogger<DeletePersonalActionCommandHandler> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Unit> Handle(DeletePersonalActionCommand request, CancellationToken cancellationToken)
        {
            // 1. Eliminar acción (validaciones y lógica de negocio encapsuladas)
            var deletedCount = await _domainService.DeleteActionAsync(request.ActionId, cancellationToken);

            // 2. Persistir (UnitOfWorkBehavior maneja la transacción automáticamente)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 3. Log
            _logger.LogInformation(
                "Acción de personal eliminada exitosamente. Id: {ActionId}, Acciones eliminadas: {Count}",
                request.ActionId,
                deletedCount);

            return Unit.Value;
        }
    }
}
