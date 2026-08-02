using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class EmbeddingChunkConfiguration : IEntityTypeConfiguration<EmbeddingChunk>
{
    public void Configure(EntityTypeBuilder<EmbeddingChunk> builder)
    {
        builder.ToTable("EmbeddingChunks");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.Metadata)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.DocumentId)
            .HasDatabaseName("IX_EmbeddingChunks_DocumentId");

        builder.HasIndex(e => new { e.DocumentId, e.ChunkIndex })
            .IsUnique()
            .HasDatabaseName("IX_EmbeddingChunks_DocumentId_ChunkIndex");

        builder.HasOne(e => e.Document)
            .WithMany()
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
