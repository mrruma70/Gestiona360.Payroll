// src/Gestiona360.Payroll.Application/Features/Employees/Commands/UpdateEmployeeCommandHandler.cs
using Gestiona360.Payroll.Application.Services;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;
using Gestiona360.Payroll.Domain.Services;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands;

/// <summary>
/// Handler orquestador para actualización de empleados.
/// Cumple SRP: Solo coordina validación, mapeo y persistencia.
/// Sin DbContext directo, sin SaveChanges manual, sin lógica de negocio inline.
/// </summary>
public class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _employeeRepo;
    private readonly EmployeeDomainService _domainService;

    public UpdateEmployeeCommandHandler(
        IEmployeeRepository employeeRepo,
        EmployeeDomainService domainService)
    {
        _employeeRepo = employeeRepo;
        _domainService = domainService;
    }

    public async Task<Unit> Handle(UpdateEmployeeCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // 1. VALIDACIONES DE ENTRADA (Campos obligatorios básicos)
        _domainService.ValidateRequiredFieldsForUpdate(
            req.Identification,
            req.DepartmentId,
            req.MunicipalityId);

        // 2. VALIDACIÓN DE CÉDULA (Formato + Unicidad excluyendo propio ID)
        // Preserva exactamente la lógica original de "No duplicar cédula en otros empleados"
        await _domainService.ValidateIdentificationForUpdateAsync(req.Id, req.Identification!, ct);

        // 3. VALIDACIÓN DE UBICACIÓN (Dept/Mun activos y compatibles)
        // Reutiliza el mismo método que CreateEmployee para consistencia
        await _domainService.ValidateLocationAsync(req.DepartmentId!.Value, req.MunicipalityId!.Value, ct);

        // 4. OBTENER ENTIDAD CON TRACKING
        // FindAsync habilita Change Tracker automáticamente para updates eficientes
        var employee = await _employeeRepo.GetByIdAsync(req.Id, ct)
            ?? throw new NotFoundException(nameof(Employee), req.Id);

        // 5. MAPEO SEGURO DTO → ENTIDAD
        // Delegado a Application Service para no contaminar Domain con DTOs
        EmployeeUpdateService.ApplyChanges(employee, req);

        // 6. PERSISTENCIA AUTOMÁTICA VÍA UNITOFWORKBEHAVIOR
        // NO llamar SaveChangesAsync aquí. El Behavior lo hace tras CommitTransaction.

        return Unit.Value;
    }
}