// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GetEmployeeByIdQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetEmployeeByIdQueryHandler : IRequestHandler<GetEmployeeByIdQuery, EmployeeDetailDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeByIdQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<EmployeeDetailDto> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetEmployeeByIdWithShiftAsync(request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new NotFoundException(nameof(Employee), request.EmployeeId);

        return EmployeeMapper.ToDetailDtoWithShift(employee);
    }
}