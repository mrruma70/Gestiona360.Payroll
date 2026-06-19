// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GetEmployeeConceptsQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetEmployeeConceptsQueryHandler : IRequestHandler<GetEmployeeConceptsQuery, EmployeeConceptsResultDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeConceptsQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<EmployeeConceptsResultDto> Handle(GetEmployeeConceptsQuery request, CancellationToken cancellationToken)
    {
        var data = await _employeeRepository.GetEmployeeConceptsAsync(request.EmployeeId, cancellationToken);

        if (data == null)
            throw new KeyNotFoundException($"Empleado con ID {request.EmployeeId} no encontrado.");

        return EmployeeConceptsMapper.ToResultDto(data);
    }
}