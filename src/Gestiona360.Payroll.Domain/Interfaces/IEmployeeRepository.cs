using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Shared.Frontend;


namespace Gestiona360.Payroll.Domain.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<IEnumerable<Employee>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct = default);
        Task<IEnumerable<Employee>> FindAsync(Expression<Func<Employee, bool>> predicate, CancellationToken ct = default);
        Task<Employee> AddAsync(Employee employee, CancellationToken ct = default);
        void Update(Employee employee);
        void Delete(Employee employee);
        Task<decimal> GetTotalSalaryByDepartmentAsync(int departmentId, CancellationToken ct = default);
        Task<Employee?> GetByIdentificationAsync(string identification, CancellationToken ct = default);

        Task<string> GenerateNextEmployeeCodeAsync(CancellationToken ct = default);

        Task<Employee?> GetEmployeeWithFullDetailsAsync(Guid id, CancellationToken cancellationToken = default);

        Task<Employee?> GetEmployeeForDetailAsync(Guid id, CancellationToken ct = default);

        Task<IEnumerable<Employee>> GetEmployeesForListAsync(
      Expression<Func<Employee, bool>>? predicate = null,
      CancellationToken ct = default);

        Task<EmployeeStatsEntity> GetEmployeeStatsAsync(CancellationToken ct = default);

        Task<Employee?> GetEmployeeByIdWithShiftAsync(Guid id, CancellationToken ct = default);

        Task<EmployeeConceptsData?> GetEmployeeConceptsAsync(Guid employeeId, CancellationToken ct = default);

        Task<EmployeeShiftsData?> GetEmployeeShiftsAsync(Guid employeeId, CancellationToken ct = default);

        Task<IEnumerable<EmployeeBasicInfo>> GetActiveEmployeesAsync(CancellationToken ct = default);

        Task<Employee?> GetEmployeeForCredentialAsync(Guid id, CancellationToken ct = default);

        Task<Employee?> GetByIdWithJobDetailsAsync(Guid id, CancellationToken ct = default);

        Task<List<ActiveEmployeeDto>> GetActiveEmployeesSummaryAsync(CancellationToken ct = default);

        Task<decimal?> GetMinimumWageForEmployeeAsync(Guid employeeId, CancellationToken ct = default);

    }
}


