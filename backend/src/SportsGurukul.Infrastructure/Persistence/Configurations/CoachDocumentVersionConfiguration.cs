using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachDocumentVersionConfiguration : IEntityTypeConfiguration<CoachDocumentVersion>
{
    public void Configure(EntityTypeBuilder<CoachDocumentVersion> builder)
    {
        builder.ToTable("CoachDocumentVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.DocumentId)
            .IsRequired();

        builder.Property(v => v.VersionNumber)
            .IsRequired();

        builder.Property(v => v.StoredFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(v => v.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(v => v.FileSize)
            .IsRequired();

        builder.Property(v => v.Checksum)
            .HasMaxLength(255);

        builder.Property(v => v.UploadedBy)
            .IsRequired()
            .HasMaxLength(450);

        builder.Ignore(v => v.CreatedBy);
        builder.Ignore(v => v.UpdatedBy);

        builder.HasOne(v => v.Document)
            .WithMany(d => d.Versions)
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(v => v.DocumentId)
            .HasDatabaseName("IX_CoachDocumentVersions_DocumentId");
    }
}
