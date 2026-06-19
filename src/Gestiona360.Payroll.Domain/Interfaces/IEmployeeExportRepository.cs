

using Gestiona360.Payroll.Domain.Shared.Frontend;

namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IEmployeeExportRepository
    {
        Task<IEnumerable<EmployeeExportDto>> GetEmployeesForExportAsync(
            EmployeeExportFilters filters,
            CancellationToken ct = default);
    }
}
