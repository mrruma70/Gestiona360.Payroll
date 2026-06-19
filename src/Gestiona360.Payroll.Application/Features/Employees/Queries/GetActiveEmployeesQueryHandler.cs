// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GetActiveEmployeesQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs;
using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetActiveEmployeesQueryHandler : IRequestHandler<GetActiveEmployeesQuery, List<EmployeeDto>>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetActiveEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<List<EmployeeDto>> Handle(GetActiveEmployeesQuery request, CancellationToken cancellationToken)
    {
        var employees = await _employeeRepository.GetActiveEmployeesAsync(cancellationToken);

        return employees.Select(EmployeeMapper.ToDto).ToList();
    }
}