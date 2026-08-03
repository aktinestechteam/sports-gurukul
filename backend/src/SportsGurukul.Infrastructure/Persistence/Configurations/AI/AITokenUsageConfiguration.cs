using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AITokenUsageConfiguration : IEntityTypeConfiguration<AITokenUsage>
{
    public void Configure(EntityTypeBuilder<AITokenUsage> builder)
    {
        builder.ToTable("AITokenUsages");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.UsageType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(t => t.Cost)
            .HasPrecision(18, 6);

        builder.Property(t => t.Currency)
            .HasMaxLength(10);

        builder.Property(t => t.ModelName)
            .HasMaxLength(150);

        builder.Property(t => t.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(t => t.ProviderId)
            .HasDatabaseName("IX_AITokenUsages_ProviderId");

        builder.HasIndex(t => t.ModelId)
            .HasDatabaseName("IX_AITokenUsages_ModelId");

        builder.HasIndex(t => t.AssistantId)
            .HasDatabaseName("IX_AITokenUsages_AssistantId");

        builder.HasIndex(t => t.ConversationId)
            .HasDatabaseName("IX_AITokenUsages_ConversationId");

        builder.HasIndex(t => t.UserId)
            .HasDatabaseName("IX_AITokenUsages_UserId");

        builder.HasIndex(t => t.UsageType)
            .HasDatabaseName("IX_AITokenUsages_UsageType");

        builder.HasIndex(t => t.CreatedAt)
            .HasDatabaseName("IX_AITokenUsages_CreatedAt");

        builder.HasIndex(t => new { t.ModelId, t.CreatedAt })
            .HasDatabaseName("IX_AITokenUsages_ModelId_CreatedAt");

        builder.HasOne(t => t.Provider)
            .WithMany(p => p.TokenUsages)
            .HasForeignKey(t => t.ProviderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Model)
            .WithMany(m => m.TokenUsages)
            .HasForeignKey(t => t.ModelId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Assistant)
            .WithMany(a => a.TokenUsages)
            .HasForeignKey(t => t.AssistantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(t => t.Conversation)
            .WithMany(c => c.TokenUsages)
            .HasForeignKey(t => t.ConversationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.Ignore(t => t.CreatedBy);
        builder.Ignore(t => t.UpdatedBy);
    }
}
