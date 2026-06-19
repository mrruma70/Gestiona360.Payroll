using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gestiona360.Payroll.Domain.Interfaces;

namespace Gestiona360.Payroll.Application.Abstractions.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IBranchRepository Branches { get; }
        ICompanyRepository Companies { get; }
        IInatecConfigRepository InatecConfigs { get; }
        IInssConfigRepository InssConfigs { get; }
        IPayrollGroupRepository PayrollGroups { get; }
        IPersonalActionRepository PersonalActions { get; }
        IPayrollPeriodRepository PayrollPeriods { get; }  
        IEmployeeShiftAssignmentRepository EmployeeShiftAssignments { get; }
        IMinimumWageRepository MinimumWages { get; }

        //IPayrollRepository Payrolls { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

}
