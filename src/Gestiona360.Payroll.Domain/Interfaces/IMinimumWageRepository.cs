// IMinimumWageRepository.cs
using Gestiona360.Payroll.Domain.Entities;
namespace Gestiona360.Payroll.Domain.Interfaces;

public interface IMinimumWageRepository
{
    Task<MinimumWage?> GetByIdAsync(int id, CancellationToken ct);
}