using System;
using System.Collections.Generic;
using System.Text;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IPersonalActionValidationService
    {
        Task<bool> IsSalaryAboveMinimumAsync(Guid employeeId, decimal newSalary, CancellationToken ct);
        Task<bool> IsNewJobGradeDifferentAsync(Guid employeeId, Guid? newJobGradeId, CancellationToken ct);
        Task<bool> IsNewJobGradeSalaryValidAsync(Guid employeeId, Guid? newJobGradeId, CancellationToken ct);
        Task<bool> IsNewContractTypeDifferentAsync(Guid employeeId, int? newContractTypeId, CancellationToken ct);
        Task<bool> IsNewShiftDifferentAsync(Guid employeeId, Guid? newShiftId, CancellationToken ct);
        Task<bool> IsNewCostCenterDifferentAsync(Guid employeeId, Guid? newCostCenterId, CancellationToken ct);
        Task<bool> IsNewHealthProviderDifferentAsync(Guid employeeId, Guid? newHealthProviderId, CancellationToken ct);
        Task<bool> IsNewBankAccountDifferentAsync(Guid employeeId, int? newBankId, string? newAccountNumber, CancellationToken ct);
        Task<bool> HasNoActiveSuspensionAsync(Guid employeeId, CancellationToken ct);
        Task<bool> IsEmployeeSuspendedAsync(Guid employeeId, CancellationToken ct);
        Task<bool> CanBeTerminatedAsync(Guid employeeId, CancellationToken ct);
    }
}
