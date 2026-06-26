using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Features.PersonalActions.Strategies;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Services;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.PersonalActions.Commands.ExecutePersonalAction;

public class ExecutePersonalActionCommandHandler : IRequestHandler<ExecutePersonalActionCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly PersonalActionDomainService _domainService;
    private readonly IEnumerable<IPersonalActionStrategy> _strategies;

    public ExecutePersonalActionCommandHandler(
        IUnitOfWork unitOfWork,
        PersonalActionDomainService domainService,
        IEnumerable<IPersonalActionStrategy> strategies)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
        _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
    }

    public async Task<Unit> Handle(ExecutePersonalActionCommand request, CancellationToken ct)
    {
        // Ejecutar acción (validaciones, estrategia y actualización encapsuladas)
        await _domainService.ExecuteActionAsync(request.ActionId, _strategies, ct);

        // Persistir (UnitOfWorkBehavior maneja la transacción automáticamente)
        await _unitOfWork.SaveChangesAsync(ct);

        return Unit.Value;
    }
}