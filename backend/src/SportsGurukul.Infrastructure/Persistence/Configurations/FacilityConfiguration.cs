using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityConfiguration : IEntityTypeConfiguration<Facility>
{
    public void Configure(EntityTypeBuilder<Facility> builder)
    {
        builder.ToTable("Facilities");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.FacilityCode)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(f => f.FacilityName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(f => f.FacilityType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(f => f.Description)
            .HasMaxLength(2000);

        builder.Property(f => f.IndoorOutdoor)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(f => f.SurfaceType)
            .HasMaxLength(100);

        builder.Property(f => f.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(f => f.RowVersion)
            .IsRowVersion();

        builder.HasIndex(f => f.FacilityCode)
            .IsUnique()
            .HasDatabaseName("IX_Facilities_FacilityCode");

        builder.HasIndex(f => f.AcademyId)
            .HasDatabaseName("IX_Facilities_AcademyId");

        builder.HasIndex(f => f.BranchId)
            .HasDatabaseName("IX_Facilities_BranchId");

        builder.HasIndex(f => f.FacilityType)
            .HasDatabaseName("IX_Facilities_FacilityType");

        builder.HasIndex(f => f.Status)
            .HasDatabaseName("IX_Facilities_Status");

        builder.HasIndex(f => new { f.AcademyId, f.FacilityType })
            .HasDatabaseName("IX_Facilities_AcademyId_FacilityType");

        builder.HasIndex(f => new { f.AcademyId, f.BranchId, f.FacilityName })
            .IsUnique()
            .HasDatabaseName("IX_Facilities_AcademyId_BranchId_Name");

        builder.HasOne(f => f.Academy)
            .WithMany()
            .HasForeignKey(f => f.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.Branch)
            .WithMany()
            .HasForeignKey(f => f.BranchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(f => f.CreatedBy);
        builder.Ignore(f => f.UpdatedBy);
    }
}
