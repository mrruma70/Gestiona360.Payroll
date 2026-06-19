// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GenerateEmployeePdfQueryHandler.cs

using Gestiona360.Payroll.Application.Features.Employees.Reports;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;
using QuestPDF.Fluent;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

public class GenerateEmployeePdfQueryHandler : IRequestHandler<GenerateEmployeePdfQuery, byte[]>
{
    private readonly IEmployeeRepository _employeeRepository;

    public GenerateEmployeePdfQueryHandler(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<byte[]> Handle(GenerateEmployeePdfQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetEmployeeWithFullDetailsAsync(request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new KeyNotFoundException($"No se encontró el empleado con ID {request.EmployeeId}.");

        var document = new EmployeeFichaDocument(employee, request.WebRootPath);
        return document.GeneratePdf();
    }
}