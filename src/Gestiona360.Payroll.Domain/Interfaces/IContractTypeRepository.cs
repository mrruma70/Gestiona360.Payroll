// IContractTypeRepository.cs
using Gestiona360.Payroll.Domain.Entities;
namespace Gestiona360.Payroll.Domain.Interfaces;

public interface IContractTypeRepository
{
    Task<ContractType?> GetByIdAsync(int id, CancellationToken ct);
}