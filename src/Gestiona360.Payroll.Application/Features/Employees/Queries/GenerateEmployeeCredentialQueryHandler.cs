// src/Gestiona360.Payroll.Application/Features/Employees/Queries/GenerateEmployeeCredentialQueryHandler.cs

using Gestiona360.Payroll.Application.Features.Employees.Reports;
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;
using QuestPDF.Fluent;

namespace Gestiona360.Payroll.Application.Features.Employees.Queries;

/// <summary>
/// Handler para generar el PDF de credencial tipo carnet.
/// </summary>
public class GenerateEmployeeCredentialQueryHandler : IRequestHandler<GenerateEmployeeCredentialQuery, byte[]>
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly EmployeeBarcodeService _barcodeService;

    public GenerateEmployeeCredentialQueryHandler(
        IEmployeeRepository employeeRepository,
        EmployeeBarcodeService barcodeService)
    {
        _employeeRepository = employeeRepository ?? throw new ArgumentNullException(nameof(employeeRepository));
        _barcodeService = barcodeService ?? throw new ArgumentNullException(nameof(barcodeService));
    }

    public async Task<byte[]> Handle(GenerateEmployeeCredentialQuery request, CancellationToken cancellationToken)
    {
        var employee = await _employeeRepository.GetEmployeeForCredentialAsync(request.EmployeeId, cancellationToken);

        if (employee == null)
            throw new NotFoundException(nameof(Employee), request.EmployeeId);

        var document = new EmployeeCredentialDocument(employee, request.WebRootPath, _barcodeService);
        return document.GeneratePdf();
    }
}