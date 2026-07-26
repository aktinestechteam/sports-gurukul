using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityEquipmentConfiguration : IEntityTypeConfiguration<FacilityEquipment>
{
    public void Configure(EntityTypeBuilder<FacilityEquipment> builder)
    {
        builder.ToTable("FacilityEquipment");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EquipmentName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Category)
            .HasMaxLength(100);

        builder.Property(e => e.MaintenanceSchedule)
            .HasMaxLength(500);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.Condition)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(e => e.FacilityId)
            .HasDatabaseName("IX_FacilityEquipment_FacilityId");

        builder.HasIndex(e => new { e.FacilityId, e.EquipmentName })
            .HasDatabaseName("IX_FacilityEquipment_FacilityId_Name");

        builder.HasOne(e => e.Facility)
            .WithMany(f => f.Equipment)
            .HasForeignKey(e => e.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
