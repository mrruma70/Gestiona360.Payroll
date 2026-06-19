// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GetEmployeePersonalActionsQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Features.PersonalActions.Services;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetEmployeePersonalActionsQueryHandler : IRequestHandler<GetEmployeePersonalActionsQuery, List<PersonalActionDto>>
{
    private readonly IPersonalActionRepository _personalActionRepository;

    public GetEmployeePersonalActionsQueryHandler(IPersonalActionRepository personalActionRepository)
    {
        _personalActionRepository = personalActionRepository ?? throw new ArgumentNullException(nameof(personalActionRepository));
    }

    public async Task<List<PersonalActionDto>> Handle(GetEmployeePersonalActionsQuery request, CancellationToken cancellationToken)
    {
        var actions = await _personalActionRepository.GetPersonalActionsWithDetailsAsync(request.EmployeeId, cancellationToken);

        return actions.Select(PersonalActionMapper.ToDto).ToList();
    }
}
