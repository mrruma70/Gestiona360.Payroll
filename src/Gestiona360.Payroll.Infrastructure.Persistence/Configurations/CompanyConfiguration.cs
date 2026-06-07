using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gestiona360.Payroll.Domain.Entities;

namespace Gestiona360.Payroll.Infrastructure.Persistence.Configurations
{
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");

            // Clave primaria (sin UseIdentityColumn para Guid)
            builder.HasKey(c => c.Id);
            //builder.Property(c => c.Id).UseIdentityColumn(); // autoincrement

            // Índices únicos y de búsqueda
            builder.HasIndex(c => c.TaxId).IsUnique();
            builder.HasIndex(c => c.LegalName);
            builder.HasIndex(c => c.CommercialName);
            builder.HasIndex(c => c.INSSEmployerCode);

            // Propiedades requeridas y longitudes
            builder.Property(c => c.LegalName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.CommercialName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.TaxId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.INSSEmployerCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.EconomicActivityCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(c => c.Phone)
                .HasMaxLength(20);

            builder.Property(c => c.Email)
                .HasMaxLength(100);

            builder.Property(c => c.Address)
                .HasMaxLength(200);

            builder.Property(c => c.City)
                .HasMaxLength(50);

            builder.Property(c => c.Department)
                .HasMaxLength(50);

            builder.Property(c => c.LogoUrl)
                .HasMaxLength(500);

            builder.Property(c => c.LegalRepresentative)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.LegalRepresentativeId)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(c => c.DefaultCurrency)
                .IsRequired()
                .HasMaxLength(3)
                .HasDefaultValue("NIO");

            // Relaciones
            builder.HasOne(c => c.DefaultPayrollFrequency)
                .WithMany()
                .HasForeignKey(c => c.DefaultPayrollFrequencyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.Branches)
     .WithOne(b => b.Company)
     .HasForeignKey(b => b.CompanyId)
     .OnDelete(DeleteBehavior.Restrict);   // Cambiado de Cascade

            // Conversiones y valores por defecto
            builder.Property(c => c.IsActive)
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Ignorar propiedades no mapeadas si las hubiera
            // builder.Ignore(c => c.SomeUnmappedProperty);
        }
    }
}