using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class SemanticSearchRequestConfiguration : IEntityTypeConfiguration<SemanticSearchRequest>
{
    public void Configure(EntityTypeBuilder<SemanticSearchRequest> builder)
    {
        builder.ToTable("SemanticSearchRequests");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Query)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(e => e.ModelName)
            .HasMaxLength(200);

        builder.Property(e => e.Filters)
            .HasMaxLength(4000);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_SemanticSearchRequests_Status");

        builder.HasIndex(e => e.KnowledgeBaseId)
            .HasDatabaseName("IX_SemanticSearchRequests_KnowledgeBaseId");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
