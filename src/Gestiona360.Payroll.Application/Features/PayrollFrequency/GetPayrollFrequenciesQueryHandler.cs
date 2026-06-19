using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Queries;
using MediatR;

namespace Gestiona360.Payroll.Application.Handlers.PayrollFrequency
{
    public class GetPayrollFrequenciesQueryHandler : IRequestHandler<GetPayrollFrequenciesQuery, List<PayrollFrequencyDto>>
    {
        private readonly IPayrollFrequencyRepository _repository;

        public GetPayrollFrequenciesQueryHandler(IPayrollFrequencyRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<PayrollFrequencyDto>> Handle(GetPayrollFrequenciesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllActiveAsync(cancellationToken);

            return entities.Select(entity => new PayrollFrequencyDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Code = entity.Code,
                Description = entity.Description,
                DaysPerPeriod = entity.DaysPerPeriod,
                PeriodsPerYear = entity.PeriodsPerYear,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt
            }).ToList();
        }
    }
}
