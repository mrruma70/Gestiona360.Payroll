// src/Gestiona360.Payroll.Infrastructure.Persistence/Repositories/UnitOfWork.cs

using Gestiona360.Payroll.Application.Abstractions.Repositories;
using Gestiona360.Payroll.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IEmployeeRepository? _employees;
    private IBranchRepository? _branches;
    private ICompanyRepository? _companies;
    private IInatecConfigRepository? _inatecConfigs;
    private IInssConfigRepository? _inssConfigs;
    private IPayrollGroupRepository? _payrollGroups;
    private IPersonalActionRepository? _personalActions;
    private IPayrollPeriodRepository? _payrollPeriods;  
    private IEmployeeShiftAssignmentRepository? _employeeShiftAssignments;
    private IMinimumWageRepository? _minimumWages;


    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEmployeeRepository Employees =>
        _employees ??= new EmployeeRepository(_context);

    public IBranchRepository Branches =>
        _branches ??= new BranchRepository(_context);

    public ICompanyRepository Companies =>
        _companies ??= new CompanyRepository(_context);

    public IInatecConfigRepository InatecConfigs => _inatecConfigs ??= new InatecConfigRepository(_context);
    public IInssConfigRepository InssConfigs => _inssConfigs ??= new InssConfigRepository(_context);
    public IPayrollGroupRepository PayrollGroups => _payrollGroups ??= new PayrollGroupRepository(_context);
    public IPersonalActionRepository PersonalActions => _personalActions ??= new PersonalActionRepository(_context);

    public IPayrollPeriodRepository PayrollPeriods => _payrollPeriods ??= new PayrollPeriodRepository(_context);  
    public IEmployeeShiftAssignmentRepository EmployeeShiftAssignments => _employeeShiftAssignments ??= new EmployeeShiftAssignmentRepository(_context);
    public IMinimumWageRepository MinimumWages => _minimumWages ??= new MinimumWageRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        if (_transaction != null)
            throw new InvalidOperationException("Ya existe una transacción activa.");

        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction == null)
            throw new InvalidOperationException("No hay transacción activa para confirmar.");

        try
        {
            await SaveChangesAsync();
            await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            // ✅ LIMPIAR: La transacción ya no puede reutilizarse
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            try
            {
                await _transaction.RollbackAsync();
            }
            finally
            {
                // ✅ LIMPIAR: La transacción ya no puede reutilizarse
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
        _context.Dispose();
    }
}