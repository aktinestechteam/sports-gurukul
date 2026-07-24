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

        builder.Property(d => d.CoachId)
            .IsRequired();

        builder.Property(d => d.Category)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Description)
            .HasMaxLength(1000);

        builder.Property(d => d.OriginalFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.StoredFileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(d => d.StorageProvider)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(d => d.StoragePath)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(d => d.MimeType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(d => d.Extension)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(d => d.FileSize)
            .IsRequired();

        builder.Property(d => d.Checksum)
            .HasMaxLength(255);

        builder.Property(d => d.Version)
            .IsRequired();

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.UploadedBy)
            .IsRequired()
            .HasMaxLength(450);

        builder.Property(d => d.UploadedOn)
            .IsRequired();

        builder.Property(d => d.VerifiedBy)
            .HasMaxLength(450);

        builder.Property(d => d.ExpiryDate);

        builder.Property(d => d.Remarks)
            .HasMaxLength(500);

        builder.Property(d => d.IsPublic)
            .IsRequired();

        builder.Ignore(d => d.CreatedBy);
        builder.Ignore(d => d.UpdatedBy);

        builder.HasOne(d => d.Coach)
            .WithMany()
            .HasForeignKey(d => d.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.Versions)
            .WithOne(v => v.Document)
            .HasForeignKey(v => v.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(d => d.AuditTrail)
            .WithOne(a => a.Document)
            .HasForeignKey(a => a.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

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
    }
}
