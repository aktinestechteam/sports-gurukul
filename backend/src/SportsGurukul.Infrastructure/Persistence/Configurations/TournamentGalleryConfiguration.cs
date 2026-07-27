using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentGalleryConfiguration : IEntityTypeConfiguration<TournamentGallery>
{
    public void Configure(EntityTypeBuilder<TournamentGallery> builder)
    {
        builder.ToTable("TournamentGallery");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.MediaType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(g => g.MediaUrl)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(g => g.Caption)
            .HasMaxLength(500);

        builder.Property(g => g.Description)
            .HasMaxLength(1000);

        builder.Property(g => g.ThumbnailUrl)
            .HasMaxLength(500);

        builder.Property(g => g.RowVersion)
            .IsRowVersion();

        builder.HasIndex(g => g.TournamentId)
            .HasDatabaseName("IX_TournamentGallery_TournamentId");

        builder.HasIndex(g => g.MediaType)
            .HasDatabaseName("IX_TournamentGallery_MediaType");

        builder.HasOne(g => g.Tournament)
            .WithMany(t => t.Gallery)
            .HasForeignKey(g => g.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(g => g.CreatedBy);
        builder.Ignore(g => g.UpdatedBy);
    }
}
