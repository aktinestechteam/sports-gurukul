using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityAreaConfiguration : IEntityTypeConfiguration<FacilityArea>
{
    public void Configure(EntityTypeBuilder<FacilityArea> builder)
    {
        builder.ToTable("FacilityAreas");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AreaName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.AreaType)
            .HasMaxLength(50);

        builder.HasIndex(a => a.FacilityId)
            .HasDatabaseName("IX_FacilityAreas_FacilityId");

        builder.HasIndex(a => new { a.FacilityId, a.AreaName })
            .IsUnique()
            .HasDatabaseName("IX_FacilityAreas_FacilityId_Name");

        builder.HasOne(a => a.Facility)
            .WithMany(f => f.Areas)
            .HasForeignKey(a => a.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
