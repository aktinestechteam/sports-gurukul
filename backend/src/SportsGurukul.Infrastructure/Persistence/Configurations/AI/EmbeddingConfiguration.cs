using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class EmbeddingConfiguration : IEntityTypeConfiguration<Embedding>
{
    public void Configure(EntityTypeBuilder<Embedding> builder)
    {
        builder.ToTable("Embeddings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ModelName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.ChunkId)
            .IsUnique()
            .HasDatabaseName("IX_Embeddings_ChunkId");

        builder.HasIndex(e => e.KnowledgeBaseId)
            .HasDatabaseName("IX_Embeddings_KnowledgeBaseId");

        builder.HasIndex(e => e.ModelId)
            .HasDatabaseName("IX_Embeddings_ModelId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Embeddings_Status");

        builder.HasIndex(e => new { e.KnowledgeBaseId, e.Status })
            .HasDatabaseName("IX_Embeddings_KnowledgeBaseId_Status");

        builder.HasOne(e => e.Chunk)
            .WithOne(c => c.Embedding)
            .HasForeignKey<Embedding>(e => e.ChunkId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.KnowledgeBase)
            .WithMany(k => k.Embeddings)
            .HasForeignKey(e => e.KnowledgeBaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Model)
            .WithMany(m => m.Embeddings)
            .HasForeignKey(e => e.ModelId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
