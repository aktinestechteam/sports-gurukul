using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class FacilityImageConfiguration : IEntityTypeConfiguration<FacilityImage>
{
    public void Configure(EntityTypeBuilder<FacilityImage> builder)
    {
        builder.ToTable("FacilityImages");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(i => i.Caption)
            .HasMaxLength(500);

        builder.HasIndex(i => i.FacilityId)
            .HasDatabaseName("IX_FacilityImages_FacilityId");

        builder.HasOne(i => i.Facility)
            .WithMany(f => f.Images)
            .HasForeignKey(i => i.FacilityId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(i => i.CreatedBy);
        builder.Ignore(i => i.UpdatedBy);
    }
}
