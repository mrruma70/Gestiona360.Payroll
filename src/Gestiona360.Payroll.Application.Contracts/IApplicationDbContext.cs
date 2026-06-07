// src/Gestiona360.Payroll.Application.Contracts/IApplicationDbContext.cs
using Gestiona360.Payroll.Domain.Entities;
using Gestiona360.Payroll.Domain.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Gestiona360.Payroll.Application.Contracts
{
    public interface IApplicationDbContext
    {
        // DbSets para todas las entidades de dominio
        DbSet<Company> Companies { get; set; }
        DbSet<Branch> Branches { get; set; }
        DbSet<Employee> Employees { get; set; }
        DbSet<ContractType> ContractTypes { get; set; }
        DbSet<HealthProvider> HealthProviders { get; set; }
        DbSet<OccupationalRisk> OccupationalRisks { get; set; }
        DbSet<Shift> Shifts { get; set; }
        DbSet<Schedule> Schedules { get; set; }
        DbSet<EmployeeShiftAssignment> EmployeeShiftAssignments { get; set; }
        DbSet<JobPosition> JobPositions { get; set; }
        DbSet<JobGrade> JobGrades { get; set; }
        DbSet<CostCenter> CostCenters { get; set; }
        DbSet<GLAccount> GLAccounts { get; set; }
        DbSet<PayrollConceptGLMapping> PayrollConceptGLMappings { get; set; }
        DbSet<PayrollFrequency> PayrollFrequencies { get; set; }
        DbSet<PayrollGroup> PayrollGroups { get; set; }
        DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        DbSet<PayrollRecord> PayrollRecords { get; set; }
        DbSet<PayrollConcept> PayrollConcepts { get; set; }
        DbSet<EmployeeConceptSetting> EmployeeConceptSettings { get; set; }
        DbSet<PayrollConceptTransaction> PayrollConceptTransactions { get; set; }
        DbSet<AmortizationLedger> AmortizationLedgers { get; set; }
        DbSet<IR_TaxBracket> IR_TaxBrackets { get; set; }
        DbSet<Garnishment> Garnishments { get; set; }
        DbSet<Termination> Terminations { get; set; }
        DbSet<PayrollSlip> PayrollSlips { get; set; }
        DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        DbSet<PersonalAction> PersonalActions { get; set; }
        DbSet<VacationBalance> VacationBalances { get; set; }
        DbSet<ThirteenthMonth> ThirteenthMonths { get; set; }
        DbSet<IndemnityProvision> IndemnityProvisions { get; set; }
        DbSet<INATECRecord> INATECRecords { get; set; }
        DbSet<HolidayCalendar> HolidayCalendars { get; set; }
        DbSet<MaternityLeave> MaternityLeaves { get; set; }
        DbSet<ZFBenefit> ZFBenefits { get; set; }
        DbSet<MonthlyClosing> MonthlyClosings { get; set; }
        DbSet<YearEndClosing> YearEndClosings { get; set; }
        DbSet<INSSConfig> INSSConfigs { get; set; }
        DbSet<CurrencyExchange> CurrencyExchanges { get; set; }
        DbSet<MinimumWage> MinimumWages { get; set; }

        DbSet<Bank> Banks { get; set; }

        DbSet<EmployeeSuspension> EmployeeSuspensions { get; set; }

        DbSet<INATECConfig> INATECConfigs { get; set; }
        DbSet<IrTaxSchedule> IrTaxSchedules { get; set; }
        DbSet<MinimumWageSchedule> MinimumWageSchedules { get; set; }

        //DbSet<ReportDefinition> ReportDefinitions { get; set; }
        //DbSet<ReportParameter> ReportParameters { get; set; }

        // Métodos auxiliares
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        EntityEntry Entry(object entity);
        ChangeTracker ChangeTracker { get; }
    }
}
