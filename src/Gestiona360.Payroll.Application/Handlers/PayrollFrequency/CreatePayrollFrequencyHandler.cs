using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Features.PayrollFrequency;
using MediatR;

namespace Gestiona360.Payroll.Application.Handlers.PayrollFrequency;


public class CreatePayrollFrequencyHandler : IRequestHandler<CreatePayrollFrequencyCommand, PayrollFrequencyDto>
{
    private readonly IPayrollFrequencyRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePayrollFrequencyHandler(IPayrollFrequencyRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PayrollFrequencyDto> Handle(CreatePayrollFrequencyCommand request, CancellationToken cancellationToken)
    {
        var entity = new Gestiona360.Payroll.Domain.Entities.PayrollFrequency
        {
            Name = request.Name,
            Code = request.Code,
            Description = request.Description,
            DaysPerPeriod = request.DaysPerPeriod,
            PeriodsPerYear = request.PeriodsPerYear,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(entity, cancellationToken);
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
            CreatedAt = entity.CreatedAt
        };
    }
}