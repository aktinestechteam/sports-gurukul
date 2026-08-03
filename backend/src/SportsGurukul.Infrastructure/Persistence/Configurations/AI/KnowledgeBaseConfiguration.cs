using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class KnowledgeBaseConfiguration : IEntityTypeConfiguration<KnowledgeBase>
{
    public void Configure(EntityTypeBuilder<KnowledgeBase> builder)
    {
        builder.ToTable("KnowledgeBases");

        builder.HasKey(k => k.Id);

        builder.Property(k => k.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(k => k.Description)
            .HasMaxLength(1000);

        builder.Property(k => k.KnowledgeBaseType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(k => k.OwnerType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(k => k.ChunkingStrategy)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(k => k.MetadataSchemaJson)
            .HasMaxLength(8000);

        builder.Property(k => k.StatisticsJson)
            .HasMaxLength(8000);

        builder.Property(k => k.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(k => k.Name)
            .IsUnique()
            .HasDatabaseName("IX_KnowledgeBases_Name");

        builder.HasIndex(k => k.KnowledgeBaseType)
            .HasDatabaseName("IX_KnowledgeBases_KnowledgeBaseType");

        builder.HasIndex(k => k.OwnerUserId)
            .HasDatabaseName("IX_KnowledgeBases_OwnerUserId");

        builder.HasIndex(k => k.VectorIndexId)
            .HasDatabaseName("IX_KnowledgeBases_VectorIndexId");

        builder.HasIndex(k => k.EmbeddingModelId)
            .HasDatabaseName("IX_KnowledgeBases_EmbeddingModelId");

        builder.HasIndex(k => k.IsActive)
            .HasDatabaseName("IX_KnowledgeBases_IsActive");

        builder.HasOne(k => k.VectorIndex)
            .WithMany(v => v.KnowledgeBases)
            .HasForeignKey(k => k.VectorIndexId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(k => k.EmbeddingModel)
            .WithMany()
            .HasForeignKey(k => k.EmbeddingModelId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(k => !k.IsDeleted);

        builder.Ignore(k => k.CreatedBy);
        builder.Ignore(k => k.UpdatedBy);
    }
}
