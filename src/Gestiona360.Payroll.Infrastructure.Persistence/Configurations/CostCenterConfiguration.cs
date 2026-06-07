using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gestiona360.Payroll.Domain.Entities;

public class CostCenterConfiguration : IEntityTypeConfiguration<CostCenter>
{
    public void Configure(EntityTypeBuilder<CostCenter> builder)
    {
        builder.ToTable("CostCenters");

        builder.HasKey(c => c.Id);

        // Solución: Vinculamos explícitamente la propiedad de navegación inversa 'CostCenters'
        builder.HasOne(c => c.Branch)
               .WithMany(b => b.CostCenters) // <-- Agrega 'b => b.CostCenters' aquí
               .HasForeignKey(c => c.BranchId)
               .OnDelete(DeleteBehavior.Restrict);

        // Relación auto-referenciada (jerarquía)
        builder.HasOne(c => c.Parent)
            .WithMany(c => c.Children)
            .HasForeignKey(c => c.ParentCostCenterId)
            .OnDelete(DeleteBehavior.Restrict); // ← MUY IMPORTANTE: NO CASCADE

        // Opcional: índice para búsquedas
        builder.HasIndex(c => c.Code);
        builder.HasIndex(c => c.ParentCostCenterId);
        builder.HasIndex(c => c.BranchId);
    }
}