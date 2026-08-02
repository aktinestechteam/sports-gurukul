using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class SemanticSearchResultConfiguration : IEntityTypeConfiguration<SemanticSearchResult>
{
    public void Configure(EntityTypeBuilder<SemanticSearchResult> builder)
    {
        builder.ToTable("SemanticSearchResults");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.DocumentTitle)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(e => e.ChunkContent)
            .HasMaxLength(8000);

        builder.Property(e => e.Metadata)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.SearchRequestId)
            .HasDatabaseName("IX_SemanticSearchResults_SearchRequestId");

        builder.HasIndex(e => e.DocumentId)
            .HasDatabaseName("IX_SemanticSearchResults_DocumentId");

        builder.HasOne(e => e.SearchRequest)
            .WithMany(r => r.Results)
            .HasForeignKey(e => e.SearchRequestId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Document)
            .WithMany()
            .HasForeignKey(e => e.DocumentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
