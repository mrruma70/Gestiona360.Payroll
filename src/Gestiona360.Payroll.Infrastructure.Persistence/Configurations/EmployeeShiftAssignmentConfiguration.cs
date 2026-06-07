using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Gestiona360.Payroll.Domain.Entities;

public class EmployeeShiftAssignmentConfiguration : IEntityTypeConfiguration<EmployeeShiftAssignment>
{
    public void Configure(EntityTypeBuilder<EmployeeShiftAssignment> builder)
    {
        builder.ToTable("EmployeeShiftAssignments");
        builder.HasKey(e => e.Id);

        builder.HasOne(e => e.Employee)
            .WithMany(e => e.ShiftAssignments)
            .HasForeignKey(e => e.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Shift)
            .WithMany(s => s.Assignments)
            .HasForeignKey(e => e.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.PersonalAction)
            .WithMany()   // Si PersonalAction tiene colección de EmployeeShiftAssignment, ajusta
            .HasForeignKey(e => e.LinkedToPersonalActionId)
            .OnDelete(DeleteBehavior.Restrict);

        // Si LinkedToPersonalActionId es otra relación, también Restrict
        // builder.HasOne(e => e.LinkedPersonalAction) ... .OnDelete(DeleteBehavior.Restrict);
    }
}