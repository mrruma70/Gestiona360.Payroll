// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GetEmployeesQueryHandler.cs

using Gestiona360.Payroll.Application.Contracts.DTOs.Employees;
using Gestiona360.Payroll.Application.Mappers;
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, EmployeeSearchResultDto>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GetEmployeesQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
    }

    public async Task<EmployeeSearchResultDto> Handle(
        GetEmployeesQuery request,
        CancellationToken cancellationToken)
    {
        // ✅ 1. Construir predicado de filtros
        var predicate = EmployeeFilterService.BuildPredicate(request);

        // ✅ 2. Obtener empleados con filtros aplicados
        var employees = await _employeeRepository.GetEmployeesForListAsync(predicate, cancellationToken);

        // ✅ 3. Obtener estadísticas
        var statsEntity = await _employeeRepository.GetEmployeeStatsAsync(cancellationToken);

        // ✅ 4. Mapear resultados
        return new EmployeeSearchResultDto
        {
            Employees = employees.Select(EmployeeMapper.ToListDto).ToList(),
            Stats = EmployeeMapper.ToStatsDto(statsEntity)
        };
    }
}