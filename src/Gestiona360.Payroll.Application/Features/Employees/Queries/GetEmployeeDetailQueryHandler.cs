// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GetEmployeeDetailQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetEmployeeDetailQueryHandler : IRequestHandler<GetEmployeeDetailQuery, EmployeeDetailDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeDetailQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<EmployeeDetailDto> Handle(GetEmployeeDetailQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetEmployeeForDetailAsync(request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new NotFoundException(nameof(Employee), request.EmployeeId);

        return EmployeeMapper.ToDetailDto(employee);
    }
}