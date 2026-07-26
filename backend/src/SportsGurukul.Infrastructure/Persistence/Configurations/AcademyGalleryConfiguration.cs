using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyGalleryConfiguration : IEntityTypeConfiguration<AcademyGallery>
{
    public void Configure(EntityTypeBuilder<AcademyGallery> builder)
    {
        builder.ToTable("AcademyGalleries");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(g => g.Description)
            .HasMaxLength(1000);

        builder.Property(g => g.ImageUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(g => g.ThumbnailUrl)
            .HasMaxLength(500);

        builder.HasIndex(g => g.AcademyId)
            .HasDatabaseName("IX_AcademyGalleries_AcademyId");

        builder.HasIndex(g => new { g.AcademyId, g.SortOrder })
            .HasDatabaseName("IX_AcademyGalleries_AcademyId_SortOrder");

        builder.HasIndex(g => new { g.AcademyId, g.IsFeatured })
            .HasDatabaseName("IX_AcademyGalleries_AcademyId_IsFeatured");

        builder.HasOne(g => g.Academy)
            .WithMany(a => a.GalleryImages)
            .HasForeignKey(g => g.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(g => g.CreatedBy);
        builder.Ignore(g => g.UpdatedBy);
    }
}
