using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AcademyDocumentConfiguration : IEntityTypeConfiguration<AcademyDocument>
{
    public void Configure(EntityTypeBuilder<AcademyDocument> builder)
    {
        builder.ToTable("AcademyDocuments");

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

        builder.HasIndex(d => d.AcademyId)
            .HasDatabaseName("IX_AcademyDocuments_AcademyId");

        builder.HasIndex(d => d.Category)
            .HasDatabaseName("IX_AcademyDocuments_Category");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_AcademyDocuments_Status");

        builder.HasIndex(d => d.UploadedOn)
            .HasDatabaseName("IX_AcademyDocuments_UploadedOn");

        builder.HasIndex(d => new { d.AcademyId, d.Category })
            .HasDatabaseName("IX_AcademyDocuments_AcademyId_Category");

        builder.HasIndex(d => new { d.AcademyId, d.IsDeleted })
            .HasDatabaseName("IX_AcademyDocuments_AcademyId_IsDeleted");

        builder.HasQueryFilter(d => !d.IsDeleted);

        builder.HasOne(d => d.Academy)
            .WithMany(a => a.Documents)
            .HasForeignKey(d => d.AcademyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(d => d.CreatedBy);
        builder.Ignore(d => d.UpdatedBy);
    }
}
