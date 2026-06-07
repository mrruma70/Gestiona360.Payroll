using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.Id);

            // Índices
            builder.HasIndex(e => e.Identification).IsUnique();
            builder.HasIndex(e => e.Email);
            builder.HasIndex(e => new { e.CompanyId, e.BranchId });

            // Propiedades
            builder.Property(e => e.Identification).IsRequired().HasMaxLength(20);
            builder.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.LastName).IsRequired().HasMaxLength(50);
            builder.Property(e => e.Email).HasMaxLength(100);
            builder.Property(e => e.Phone).HasMaxLength(20);

            // Relaciones (TODAS con Restrict para evitar múltiples rutas de cascada)
            builder.HasOne(e => e.Company)
                .WithMany(c => c.Employees)
                .HasForeignKey(e => e.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Branch)
                .WithMany(b => b.Employees)
                .HasForeignKey(e => e.BranchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.ContractType)
                .WithMany()
                .HasForeignKey(e => e.ContractTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.JobGrade)
                .WithMany(jg => jg.Employees)
                .HasForeignKey(e => e.JobGradeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.HealthProvider)
                .WithMany(hp => hp.Employees)
                .HasForeignKey(e => e.HealthProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.OccupationalRisk)
                .WithMany()
                .HasForeignKey(e => e.OccupationalRiskId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}