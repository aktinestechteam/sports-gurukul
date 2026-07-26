using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyFacilityConfiguration : IEntityTypeConfiguration<AcademyFacility>
{
    public void Configure(EntityTypeBuilder<AcademyFacility> builder)
    {
        builder.ToTable("AcademyFacilities");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FacilityName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.FacilityType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(f => f.IndoorOutdoor)
            .HasMaxLength(20);

        builder.Property(f => f.Description)
            .HasMaxLength(1000);

        builder.HasIndex(f => f.AcademyId)
            .HasDatabaseName("IX_AcademyFacilities_AcademyId");

        builder.HasIndex(f => f.FacilityType)
            .HasDatabaseName("IX_AcademyFacilities_FacilityType");

        builder.HasIndex(f => new { f.AcademyId, f.FacilityType })
            .HasDatabaseName("IX_AcademyFacilities_AcademyId_FacilityType");

        builder.HasOne(f => f.Academy)
            .WithMany(a => a.Facilities)
            .HasForeignKey(f => f.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(f => f.CreatedBy);
        builder.Ignore(f => f.UpdatedBy);
    }
}
