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

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Query)
            .IsRequired();

        builder.Property(r => r.FiltersJson)
            .HasMaxLength(8000);

        builder.Property(r => r.ModelUsed)
            .HasMaxLength(150);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.RequestedByType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(r => r.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(r => r.KnowledgeBaseId)
            .HasDatabaseName("IX_SemanticSearchRequests_KnowledgeBaseId");

        builder.HasIndex(r => r.VectorIndexId)
            .HasDatabaseName("IX_SemanticSearchRequests_VectorIndexId");

        builder.HasIndex(r => r.ConversationId)
            .HasDatabaseName("IX_SemanticSearchRequests_ConversationId");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_SemanticSearchRequests_Status");

        builder.HasIndex(r => r.CreatedAt)
            .HasDatabaseName("IX_SemanticSearchRequests_CreatedAt");

        builder.HasOne(r => r.KnowledgeBase)
            .WithMany(k => k.SearchRequests)
            .HasForeignKey(r => r.KnowledgeBaseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.VectorIndex)
            .WithMany(v => v.SearchRequests)
            .HasForeignKey(r => r.VectorIndexId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Conversation)
            .WithMany(c => c.SemanticSearchRequests)
            .HasForeignKey(r => r.ConversationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(r => !r.IsDeleted);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
