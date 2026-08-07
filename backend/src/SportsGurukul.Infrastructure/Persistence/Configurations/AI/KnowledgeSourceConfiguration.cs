using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class KnowledgeSourceConfiguration : IEntityTypeConfiguration<KnowledgeSource>
{
    public void Configure(EntityTypeBuilder<KnowledgeSource> builder)
    {
        builder.ToTable("KnowledgeSources");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(1000);

        builder.Property(s => s.SourceType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.Uri)
            .HasMaxLength(2000);

        builder.Property(s => s.ExternalId)
            .HasMaxLength(200);

        builder.Property(s => s.ContentType)
            .HasMaxLength(200);

        builder.Property(s => s.IngestionStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.StatusMessage)
            .HasMaxLength(2000);

        builder.Property(s => s.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(s => s.ErrorDetailsJson)
            .HasMaxLength(8000);

        builder.Property(s => s.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(s => s.KnowledgeBaseId)
            .HasDatabaseName("IX_KnowledgeSources_KnowledgeBaseId");

        builder.HasIndex(s => new { s.KnowledgeBaseId, s.Name })
            .IsUnique()
            .HasDatabaseName("IX_KnowledgeSources_KnowledgeBaseId_Name");

        builder.HasIndex(s => s.SourceType)
            .HasDatabaseName("IX_KnowledgeSources_SourceType");

        builder.HasIndex(s => s.IngestionStatus)
            .HasDatabaseName("IX_KnowledgeSources_IngestionStatus");

        builder.HasIndex(s => s.IsActive)
            .HasDatabaseName("IX_KnowledgeSources_IsActive");

        builder.HasOne(s => s.KnowledgeBase)
            .WithMany(k => k.Sources)
            .HasForeignKey(s => s.KnowledgeBaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => !s.IsDeleted);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
