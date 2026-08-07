using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class KnowledgeDocumentConfiguration : IEntityTypeConfiguration<KnowledgeDocument>
{
    public void Configure(EntityTypeBuilder<KnowledgeDocument> builder)
    {
        builder.ToTable("KnowledgeDocuments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(d => d.DocumentType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(d => d.ContentHash)
            .HasMaxLength(128)
            .IsRequired();

        builder.Property(d => d.ExternalId)
            .HasMaxLength(200);

        builder.Property(d => d.StoragePath)
            .HasMaxLength(2000);

        builder.Property(d => d.MimeType)
            .HasMaxLength(200);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(d => d.ProcessedBy)
            .HasMaxLength(200);

        builder.Property(d => d.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(d => d.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(d => d.KnowledgeBaseId)
            .HasDatabaseName("IX_KnowledgeDocuments_KnowledgeBaseId");

        builder.HasIndex(d => d.KnowledgeSourceId)
            .HasDatabaseName("IX_KnowledgeDocuments_KnowledgeSourceId");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_KnowledgeDocuments_Status");

        builder.HasIndex(d => d.ContentHash)
            .HasDatabaseName("IX_KnowledgeDocuments_ContentHash");

        builder.HasIndex(d => new { d.KnowledgeBaseId, d.Status })
            .HasDatabaseName("IX_KnowledgeDocuments_KnowledgeBaseId_Status");

        builder.HasOne(d => d.KnowledgeBase)
            .WithMany(k => k.Documents)
            .HasForeignKey(d => d.KnowledgeBaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.KnowledgeSource)
            .WithMany(s => s.Documents)
            .HasForeignKey(d => d.KnowledgeSourceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(d => !d.IsDeleted);

        builder.Ignore(d => d.CreatedBy);
        builder.Ignore(d => d.UpdatedBy);
    }
}
