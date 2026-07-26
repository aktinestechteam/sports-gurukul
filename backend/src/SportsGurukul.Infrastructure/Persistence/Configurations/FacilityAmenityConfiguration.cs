using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityAmenityConfiguration : IEntityTypeConfiguration<FacilityAmenity>
{
    public void Configure(EntityTypeBuilder<FacilityAmenity> builder)
    {
        builder.ToTable("FacilityAmenities");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AmenityName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(500);

        builder.HasIndex(a => a.FacilityId)
            .HasDatabaseName("IX_FacilityAmenities_FacilityId");

        builder.HasIndex(a => new { a.FacilityId, a.AmenityName })
            .IsUnique()
            .HasDatabaseName("IX_FacilityAmenities_FacilityId_Name");

        builder.HasOne(a => a.Facility)
            .WithMany(f => f.Amenities)
            .HasForeignKey(a => a.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
