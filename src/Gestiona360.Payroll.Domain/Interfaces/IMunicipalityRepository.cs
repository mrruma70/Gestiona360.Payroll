// IMunicipalityRepository.cs
using Gestiona360.Payroll.Domain.Entities;
namespace Gestiona360.Payroll.Domain.Interfaces;

public interface IMunicipalityRepository
{
    Task<Municipality?> GetByIdActiveInDepartmentAsync(int id, int departmentId, CancellationToken ct);
}