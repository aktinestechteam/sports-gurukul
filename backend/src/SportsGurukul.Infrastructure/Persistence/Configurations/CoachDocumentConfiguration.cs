using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachDocumentConfiguration : IEntityTypeConfiguration<CoachDocument>
{
    public void Configure(EntityTypeBuilder<CoachDocument> builder)
    {
        builder.ToTable("CoachDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(d => d.Description)
            .HasMaxLength(1000);

        builder.Property(d => d.OriginalFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.StoredFileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(d => d.StorageProvider)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(d => d.StoragePath)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.MimeType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(d => d.Extension)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(d => d.Category)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(d => d.Checksum)
            .HasMaxLength(64);

        builder.HasIndex(d => d.CoachId)
            .HasDatabaseName("IX_CoachDocuments_CoachId");

        builder.HasIndex(d => d.Category)
            .HasDatabaseName("IX_CoachDocuments_Category");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_CoachDocuments_Status");

        builder.HasIndex(d => d.UploadedOn)
            .HasDatabaseName("IX_CoachDocuments_UploadedOn");

        builder.HasIndex(d => new { d.CoachId, d.Category })
            .HasDatabaseName("IX_CoachDocuments_CoachId_Category");

        builder.HasIndex(d => new { d.CoachId, d.IsDeleted })
            .HasDatabaseName("IX_CoachDocuments_CoachId_IsDeleted");

        builder.HasQueryFilter(d => !d.IsDeleted);

        builder.HasOne(d => d.Coach)
            .WithMany(c => c.Documents)
            .HasForeignKey(d => d.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(d => d.CreatedBy);
        builder.Ignore(d => d.UpdatedBy);
    }
}
