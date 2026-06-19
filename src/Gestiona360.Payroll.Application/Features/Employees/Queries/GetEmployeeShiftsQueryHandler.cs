// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GetEmployeeShiftsQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetEmployeeShiftsQueryHandler : IRequestHandler<GetEmployeeShiftsQuery, EmployeeShiftsResultDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeeShiftsQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<EmployeeShiftsResultDto> Handle(GetEmployeeShiftsQuery request, CancellationToken cancellationToken)
    {
        var data = await _employeeRepository.GetEmployeeShiftsAsync(request.EmployeeId, cancellationToken);

        if (data == null)
            throw new KeyNotFoundException($"Empleado con ID {request.EmployeeId} no encontrado.");

        return EmployeeShiftsMapper.ToResultDto(data);
    }
}