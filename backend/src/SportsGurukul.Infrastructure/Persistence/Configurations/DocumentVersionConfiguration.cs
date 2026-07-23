using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class DocumentVersionConfiguration : IEntityTypeConfiguration<DocumentVersion>
{
    public void Configure(EntityTypeBuilder<DocumentVersion> builder)
    {
        builder.ToTable("DocumentVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.StoredFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(v => v.StoragePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(v => v.Checksum)
            .HasMaxLength(64);

        builder.HasIndex(v => v.DocumentId)
            .HasDatabaseName("IX_DocumentVersions_DocumentId");

        builder.HasIndex(v => new { v.DocumentId, v.VersionNumber })
            .IsUnique()
            .HasDatabaseName("IX_DocumentVersions_DocumentId_VersionNumber");

        builder.HasOne(v => v.Document)
            .WithMany(d => d.Versions)
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(v => v.CreatedBy);
        builder.Ignore(v => v.UpdatedBy);
    }
}
