using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.ToTable("Conversations");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title)
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ContextSummary)
            .HasMaxLength(4000);

        builder.Property(e => e.Metadata)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_Conversations_Status");

        builder.HasIndex(e => e.AssistantId)
            .HasDatabaseName("IX_Conversations_AssistantId");

        builder.HasIndex(e => e.UserId)
            .HasDatabaseName("IX_Conversations_UserId");

        builder.HasIndex(e => e.LastActivityAt)
            .HasDatabaseName("IX_Conversations_LastActivityAt");

        builder.HasOne(e => e.Assistant)
            .WithMany(a => a.Conversations)
            .HasForeignKey(e => e.AssistantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
