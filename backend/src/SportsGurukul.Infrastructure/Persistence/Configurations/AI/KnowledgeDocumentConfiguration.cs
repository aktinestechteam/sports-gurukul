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

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Title)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.FileName)
            .HasMaxLength(500);

        builder.Property(e => e.FilePath)
            .HasMaxLength(1000);

        builder.Property(e => e.ContentType)
            .HasMaxLength(100);

        builder.Property(e => e.Metadata)
            .HasMaxLength(4000);

        builder.Property(e => e.Checksum)
            .HasMaxLength(64);

        builder.Property(e => e.EmbeddingStatus)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Title)
            .HasDatabaseName("IX_KnowledgeDocuments_Title");

        builder.HasIndex(e => e.EmbeddingStatus)
            .HasDatabaseName("IX_KnowledgeDocuments_EmbeddingStatus");

        builder.HasIndex(e => e.Type)
            .HasDatabaseName("IX_KnowledgeDocuments_Type");

        builder.HasIndex(e => e.KnowledgeSourceId)
            .HasDatabaseName("IX_KnowledgeDocuments_KnowledgeSourceId");

        builder.HasOne(e => e.KnowledgeSource)
            .WithMany(s => s.Documents)
            .HasForeignKey(e => e.KnowledgeSourceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
