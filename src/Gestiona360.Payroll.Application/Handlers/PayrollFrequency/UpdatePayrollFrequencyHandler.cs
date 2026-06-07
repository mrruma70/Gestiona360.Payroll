using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Commands;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using MediatR;

namespace Gestiona360.Payroll.Application.Handlers.PayrollFrequency;

public class UpdatePayrollFrequencyHandler : IRequestHandler<UpdatePayrollFrequencyCommand, PayrollFrequencyDto>
{
    private readonly IPayrollFrequencyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePayrollFrequencyHandler(IPayrollFrequencyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PayrollFrequencyDto> Handle(UpdatePayrollFrequencyCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Frecuencia {request.Id} no encontrada");

        entity.Name = request.Name;
        entity.Code = request.Code;
        entity.Description = request.Description;
        entity.DaysPerPeriod = request.DaysPerPeriod;
        entity.PeriodsPerYear = request.PeriodsPerYear;

        if (request.IsActive.HasValue) entity.IsActive = request.IsActive.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        _repository.Update(entity);
        await _unitOfWork.CommitAsync(cancellationToken);

        return new PayrollFrequencyDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            DaysPerPeriod = entity.DaysPerPeriod,
            PeriodsPerYear = entity.PeriodsPerYear,
            IsActive = entity.IsActive,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }
}