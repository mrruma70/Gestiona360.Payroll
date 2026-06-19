// src/Gestiona360.Payroll.Application/Features/Employees/Commands/ToggleEmployeeStatusCommandHandler.cs

using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Interfaces;
using MediatR;

namespace Gestiona360.Payroll.Application.Features.Employees.Commands;

public class ToggleEmployeeStatusCommandHandler : IRequestHandler<ToggleEmployeeStatusCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;

    public ToggleEmployeeStatusCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(ToggleEmployeeStatusCommand request, CancellationToken cancellationToken)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(request.EmployeeId);

        if (employee == null)
            throw new KeyNotFoundException($"No se encontró el empleado con ID {request.EmployeeId}.");

        // Invertir el estado
        employee.IsActive = !employee.IsActive;
        employee.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Employees.Update(employee);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}