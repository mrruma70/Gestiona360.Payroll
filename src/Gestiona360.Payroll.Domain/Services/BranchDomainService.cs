// src/Gestiona360.Payroll.Domain/Services/BranchDomainService.cs

using Gestiona360.Payroll.Domain.Exceptions;
using Gestiona360.Payroll.Domain.Interfaces;

namespace Gestiona360.Payroll.Domain.Services;

public class BranchDomainService
{
    private readonly IBranchRepository _branchRepository;

    public BranchDomainService(IBranchRepository branchRepository)
    {
        _branchRepository = branchRepository;
    }

    /// <summary>
    /// Valida que el código de sucursal sea único (para creación).
    /// </summary>
    public async Task ValidateCodeIsUniqueAsync(string code, CancellationToken ct)
    {
        var existsCode = await _branchRepository.ExistsWithCodeAsync(code, ct);
        if (existsCode)
        {
            throw new BusinessRuleViolationException(
                $"Ya existe una sucursal con el código '{code}'.");
        }
    }

    /// <summary>
    /// Valida que el código de sucursal sea único, excluyendo la sucursal actual (para actualización).
    /// </summary>
    public async Task ValidateCodeIsUniqueForUpdateAsync(string code, Guid branchId, CancellationToken ct)
    {
        var existsCode = await _branchRepository.ExistsWithCodeExcludingAsync(code, branchId, ct);
        if (existsCode)
        {
            throw new BusinessRuleViolationException(
                $"Ya existe otra sucursal con el código '{code}'.");
        }
    }

    /// <summary>
    /// Valida que la sucursal no tenga empleados activos antes de desactivarla.
    /// </summary>
    public async Task ValidateCanDeactivateAsync(Guid branchId, string branchName, CancellationToken ct)
    {
        var hasActiveEmployees = await _branchRepository.HasActiveEmployeesAsync(branchId, ct);
        if (hasActiveEmployees)
        {
            throw new BusinessRuleViolationException(
                $"No se puede desactivar la sucursal '{branchName}' porque tiene empleados activos asignados.");
        }
    }
}