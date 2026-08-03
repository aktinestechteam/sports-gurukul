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

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(300)
            .IsRequired();

        builder.Property(c => c.Summary)
            .HasMaxLength(2000);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.ParticipantType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.KnowledgeBaseIdsJson)
            .HasMaxLength(4000);

        builder.Property(c => c.ContextMetadataJson)
            .HasMaxLength(8000);

        builder.Property(c => c.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(c => c.AssistantId)
            .HasDatabaseName("IX_Conversations_AssistantId");

        builder.HasIndex(c => c.ParticipantUserId)
            .HasDatabaseName("IX_Conversations_ParticipantUserId");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Conversations_Status");

        builder.HasIndex(c => c.LastMessageAt)
            .HasDatabaseName("IX_Conversations_LastMessageAt");

        builder.HasIndex(c => new { c.AssistantId, c.Status })
            .HasDatabaseName("IX_Conversations_AssistantId_Status");

        builder.HasOne(c => c.Assistant)
            .WithMany(a => a.Conversations)
            .HasForeignKey(c => c.AssistantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
