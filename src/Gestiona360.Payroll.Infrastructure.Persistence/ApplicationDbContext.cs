// src/Gestiona360.Payroll.Infrastructure.Persistence/ApplicationDbContext.cs
using Gestiona360.Payroll.Application.Contracts;
using Gestiona360.Payroll.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Gestiona360.Payroll.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Implementación explícita de los DbSets
        public DbSet<Company> Companies { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<ContractType> ContractTypes { get; set; }
        public DbSet<HealthProvider> HealthProviders { get; set; }
        public DbSet<OccupationalRisk> OccupationalRisks { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<EmployeeShiftAssignment> EmployeeShiftAssignments { get; set; }
        public DbSet<JobPosition> JobPositions { get; set; }
        public DbSet<JobGrade> JobGrades { get; set; }
        public DbSet<CostCenter> CostCenters { get; set; }
        public DbSet<GLAccount> GLAccounts { get; set; }
        public DbSet<PayrollConceptGLMapping> PayrollConceptGLMappings { get; set; }
        public DbSet<PayrollFrequency> PayrollFrequencies { get; set; }
        public DbSet<PayrollGroup> PayrollGroups { get; set; }
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        public DbSet<PayrollRecord> PayrollRecords { get; set; }
        public DbSet<PayrollConcept> PayrollConcepts { get; set; }
        public DbSet<EmployeeConceptSetting> EmployeeConceptSettings { get; set; }
        public DbSet<PayrollConceptTransaction> PayrollConceptTransactions { get; set; }
        public DbSet<AmortizationLedger> AmortizationLedgers { get; set; }
        public DbSet<IR_TaxBracket> IR_TaxBrackets { get; set; }
        public DbSet<Garnishment> Garnishments { get; set; }
        public DbSet<Termination> Terminations { get; set; }
        public DbSet<PayrollSlip> PayrollSlips { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<PersonalAction> PersonalActions { get; set; }
        public DbSet<VacationBalance> VacationBalances { get; set; }
        public DbSet<ThirteenthMonth> ThirteenthMonths { get; set; }
        public DbSet<IndemnityProvision> IndemnityProvisions { get; set; }
        public DbSet<INATECRecord> INATECRecords { get; set; }
        public DbSet<HolidayCalendar> HolidayCalendars { get; set; }
        public DbSet<MaternityLeave> MaternityLeaves { get; set; }
        public DbSet<ZFBenefit> ZFBenefits { get; set; }
        public DbSet<MonthlyClosing> MonthlyClosings { get; set; }
        public DbSet<YearEndClosing> YearEndClosings { get; set; }
        public DbSet<INSSConfig> INSSConfigs { get; set; }
        public DbSet<CurrencyExchange> CurrencyExchanges { get; set; }
        public DbSet<MinimumWage> MinimumWages { get; set; }
        public DbSet<INATECConfig> INATECConfigs { get; set; }
        public DbSet<IrTaxSchedule> IrTaxSchedules { get; set; }
        public DbSet<MinimumWageSchedule> MinimumWageSchedules { get; set; }

        public DbSet<Bank> Banks { get; set; }
        public DbSet<EmployeeSuspension> EmployeeSuspensions { get; set; }

        // Implementación de los métodos de la interfaz
        public new async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await base.SaveChangesAsync(cancellationToken);

        public EntityEntry Entry(object entity)
            => base.Entry(entity);

        public ChangeTracker ChangeTracker => base.ChangeTracker;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Establecer comportamiento de borrado restrictivo por defecto para todas las relaciones
            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }

            // 2. Configuración de la relación ApplicationUser -> Employee (Sobrescribimos el borrado a SetNull)
            modelBuilder.Entity<ApplicationUser>()
                .HasOne(u => u.Employee)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // 3. Solución explícita para la relación CostCenter -> Branch (Evita la columna fantasma BranchId1)
            modelBuilder.Entity<CostCenter>(entity =>
            {
                entity.HasOne(cc => cc.Branch)
                    .WithMany(b => b.CostCenters)
                    .HasForeignKey(cc => cc.BranchId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // 4. Configuración explícita de la jerarquía autorreferenciada de CostCenter (Parent -> Children)
            modelBuilder.Entity<CostCenter>(entity =>
            {
                entity.HasOne(cc => cc.Parent)
                    .WithMany(cc => cc.Children)
                    .HasForeignKey(cc => cc.ParentCostCenterId)
                    .OnDelete(DeleteBehavior.Restrict);
            });


            // Employee
            modelBuilder.Entity<Employee>()
                .Property(e => e.BenefitsInKindValue)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Employee>()
                .Property(e => e.BaseSalary)
                .HasPrecision(18, 2);

            // INSSConfig
            modelBuilder.Entity<INSSConfig>()
                .Property(e => e.RateWorker)
                .HasPrecision(5, 2);

            modelBuilder.Entity<INSSConfig>()
                .Property(e => e.RateEmployerSmall)
                .HasPrecision(5, 2);

            modelBuilder.Entity<INSSConfig>()
                .Property(e => e.RateEmployerLarge)
                .HasPrecision(5, 2);

            modelBuilder.Entity<INSSConfig>()
                .Property(e => e.MaxSalaryCap)
                .HasPrecision(18, 2);

            // INATECConfig
            modelBuilder.Entity<INATECConfig>()
                .Property(e => e.Rate)
                .HasPrecision(5, 2);

            // IR_TaxBracket
            modelBuilder.Entity<IR_TaxBracket>()
                .Property(e => e.FromAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IR_TaxBracket>()
                .Property(e => e.ToAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IR_TaxBracket>()
                .Property(e => e.FixedTax)
                .HasPrecision(18, 2);

            modelBuilder.Entity<IR_TaxBracket>()
                .Property(e => e.MarginalRate)
                .HasPrecision(5, 2);

            // MinimumWage
            modelBuilder.Entity<MinimumWage>()
                .Property(e => e.MonthlyAmountNIO)
                .HasPrecision(18, 2);

            modelBuilder.Entity<MinimumWage>()
                .Property(e => e.MonthlyAmountUSD)
                .HasPrecision(18, 2);
        

        // Configuración de cascada para IR (Si se borra la cabecera, se borran los tramos)
        modelBuilder.Entity<IrTaxSchedule>()
                .HasMany(s => s.Brackets)
                .WithOne(b => b.Schedule)
                .HasForeignKey(b => b.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configuración de cascada para Salarios Mínimos
            modelBuilder.Entity<MinimumWageSchedule>()
                .HasMany(s => s.Wages)
                .WithOne(w => w.Schedule)
                .HasForeignKey(w => w.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade);

            // 5. Aplica todas las configuraciones de entidad externas (IEntityTypeConfiguration)
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}