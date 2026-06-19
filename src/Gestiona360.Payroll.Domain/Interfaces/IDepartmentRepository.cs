
// IDepartmentRepository.cs  
using Gestiona360.Payroll.Domain.Entities;
namespace Gestiona360.Payroll.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdActiveAsync(int id, CancellationToken ct);
}