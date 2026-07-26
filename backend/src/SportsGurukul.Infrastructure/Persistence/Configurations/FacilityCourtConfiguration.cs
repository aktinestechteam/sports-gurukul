using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityCourtConfiguration : IEntityTypeConfiguration<FacilityCourt>
{
    public void Configure(EntityTypeBuilder<FacilityCourt> builder)
    {
        builder.ToTable("FacilityCourts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CourtNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.CourtName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.CourtType)
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(c => c.FacilityId)
            .HasDatabaseName("IX_FacilityCourts_FacilityId");

        builder.HasIndex(c => c.FacilityAreaId)
            .HasDatabaseName("IX_FacilityCourts_FacilityAreaId");

        builder.HasIndex(c => new { c.FacilityId, c.CourtNumber })
            .IsUnique()
            .HasDatabaseName("IX_FacilityCourts_FacilityId_CourtNumber");

        builder.HasOne(c => c.Facility)
            .WithMany(f => f.Courts)
            .HasForeignKey(c => c.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.FacilityArea)
            .WithMany(a => a.Courts)
            .HasForeignKey(c => c.FacilityAreaId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
