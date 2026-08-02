using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class EmbeddingConfiguration : IEntityTypeConfiguration<Embedding>
{
    public void Configure(EntityTypeBuilder<Embedding> builder)
    {
        builder.ToTable("Embeddings");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.ModelName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Vector)
            .IsRequired();

        builder.Property(e => e.Text)
            .HasMaxLength(8000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.ModelName)
            .HasDatabaseName("IX_Embeddings_ModelName");

        builder.HasIndex(e => e.DocumentId)
            .HasDatabaseName("IX_Embeddings_DocumentId");

        builder.HasIndex(e => e.ChunkId)
            .IsUnique()
            .HasDatabaseName("IX_Embeddings_ChunkId")
            .HasFilter("[ChunkId] IS NOT NULL");

        builder.HasOne(e => e.Document)
            .WithMany(d => d.Embeddings)
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.Chunk)
            .WithOne(c => c.Embedding)
            .HasForeignKey<Embedding>(e => e.ChunkId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
