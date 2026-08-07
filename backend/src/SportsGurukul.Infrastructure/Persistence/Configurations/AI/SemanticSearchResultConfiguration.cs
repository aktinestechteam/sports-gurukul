using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class SemanticSearchResultConfiguration : IEntityTypeConfiguration<SemanticSearchResult>
{
    public void Configure(EntityTypeBuilder<SemanticSearchResult> builder)
    {
        builder.ToTable("SemanticSearchResults");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Content)
            .HasMaxLength(8000);

        builder.Property(r => r.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(r => r.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(r => r.SemanticSearchRequestId)
            .HasDatabaseName("IX_SemanticSearchResults_SearchRequestId");

        builder.HasIndex(r => r.DocumentId)
            .HasDatabaseName("IX_SemanticSearchResults_DocumentId");

        builder.HasIndex(r => r.ChunkId)
            .HasDatabaseName("IX_SemanticSearchResults_ChunkId");

        builder.HasIndex(r => new { r.SemanticSearchRequestId, r.Rank })
            .HasDatabaseName("IX_SemanticSearchResults_Request_Rank");

        builder.HasOne(r => r.SemanticSearchRequest)
            .WithMany(q => q.Results)
            .HasForeignKey(r => r.SemanticSearchRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Document)
            .WithMany(d => d.SearchResults)
            .HasForeignKey(r => r.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Chunk)
            .WithMany(c => c.SearchResults)
            .HasForeignKey(r => r.ChunkId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
