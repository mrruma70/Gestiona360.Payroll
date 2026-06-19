// src/Gestiona360.Payroll.Application/Features/Branches/Commands/UpdateBranchCommandHandler.cs

using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Services;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Branches.Commands;

public class UpdateBranchCommandHandler : IRequestHandler<UpdateBranchCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly BranchDomainService _domainService;

    public UpdateBranchCommandHandler(
        IUnitOfWork unitOfWork,
        BranchDomainService domainService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
    }

    public async Task<Unit> Handle(UpdateBranchCommand command, CancellationToken cancellationToken)
    {
        var request = command.Request;

        // 1. Obtener sucursal
        var branch = await _unitOfWork.Branches.GetByIdAsync(command.Id, cancellationToken)
            ?? throw new NotFoundException("Branch", command.Id);

        // 2. Validar código único (si cambió)
        if (branch.Code != request.Code.ToUpper())
        {
            await _domainService.ValidateCodeIsUniqueForUpdateAsync(request.Code, command.Id, cancellationToken);
        }

        // 3. Actualizar campos
        branch.Code = request.Code.ToUpper().Trim();
        branch.Name = request.Name.Trim();
        branch.Address = request.Address.Trim();
        branch.City = request.City.Trim();
        branch.Phone = request.Phone.Trim();
        branch.ManagerEmployeeId = request.ManagerEmployeeId;
        branch.DefaultCostCenterId = request.DefaultCostCenterId;
        branch.IsZoneFranca = request.IsZoneFranca;
        branch.IsActive = request.IsActive;
        branch.UpdatedAt = DateTime.UtcNow;

        // 4. Persistir
        _unitOfWork.Branches.Update(branch);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}