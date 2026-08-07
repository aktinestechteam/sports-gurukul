using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class EmbeddingChunkConfiguration : IEntityTypeConfiguration<EmbeddingChunk>
{
    public void Configure(EntityTypeBuilder<EmbeddingChunk> builder)
    {
        builder.ToTable("EmbeddingChunks");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content)
            .IsRequired();

        builder.Property(c => c.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(c => c.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(c => c.DocumentId)
            .HasDatabaseName("IX_EmbeddingChunks_DocumentId");

        builder.HasIndex(c => c.KnowledgeBaseId)
            .HasDatabaseName("IX_EmbeddingChunks_KnowledgeBaseId");

        builder.HasIndex(c => new { c.DocumentId, c.ChunkIndex })
            .IsUnique()
            .HasDatabaseName("IX_EmbeddingChunks_Document_ChunkIndex");

        builder.HasOne(c => c.Document)
            .WithMany(d => d.Chunks)
            .HasForeignKey(c => c.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.KnowledgeBase)
            .WithMany(k => k.Chunks)
            .HasForeignKey(c => c.KnowledgeBaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
