using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Features.PayrollFrequency;
using MediatR;

namespace Gestiona360.Payroll.Application.Handlers.PayrollFrequency;

public class DeletePayrollFrequencyHandler : IRequestHandler<DeletePayrollFrequencyCommand, bool>
{
    private readonly IPayrollFrequencyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePayrollFrequencyHandler(IPayrollFrequencyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeletePayrollFrequencyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Frecuencia {request.Id} no encontrada");

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _unitOfWork.CommitAsync(cancellationToken);

        return true;
    }
}