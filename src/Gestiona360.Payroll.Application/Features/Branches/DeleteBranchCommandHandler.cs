// src/Gestiona360.Payroll.Application/Features/Branches/Commands/DeleteBranchCommandHandler.cs

using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Services;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Branches.Commands;

public class DeleteBranchCommandHandler : IRequestHandler<DeleteBranchCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BranchDomainService _domainService;

    public DeleteBranchCommandHandler(
        IUnitOfWork unitOfWork,
        BranchDomainService domainService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
    }

    public async Task<Unit> Handle(DeleteBranchCommand command, CancellationToken cancellationToken)
    {
        // 1. Obtener sucursal
        var branch = await _unitOfWork.Branches.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Branch", command.Id);

        // 2. Validar que no tenga empleados activos
        await _domainService.ValidateCanDeactivateAsync(command.Id, branch.Name, cancellationToken);

        // 3. Baja lógica (no eliminar físicamente)
        branch.IsActive = false;
        branch.UpdatedAt = DateTime.UtcNow;

        // 4. Persistir
        _unitOfWork.Branches.Update(branch);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}