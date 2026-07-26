using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class EquipmentMaintenanceConfiguration : IEntityTypeConfiguration<EquipmentMaintenance>
{
    public void Configure(EntityTypeBuilder<EquipmentMaintenance> builder)
    {
        builder.ToTable("EquipmentMaintenance");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MaintenanceType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(2000);

        builder.Property(m => m.Cost)
            .HasPrecision(18, 2);

        builder.Property(m => m.PerformedBy)
            .HasMaxLength(200);

        builder.Property(m => m.Notes)
            .HasMaxLength(2000);

        builder.HasIndex(m => m.FacilityEquipmentId)
            .HasDatabaseName("IX_EquipmentMaintenance_EquipmentId");

        builder.HasIndex(m => m.ScheduledDate)
            .HasDatabaseName("IX_EquipmentMaintenance_ScheduledDate");

        builder.HasOne(m => m.FacilityEquipment)
            .WithMany(e => e.MaintenanceRecords)
            .HasForeignKey(m => m.FacilityEquipmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);
    }
}
