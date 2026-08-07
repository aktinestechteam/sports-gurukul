using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class ConversationMessageConfiguration : IEntityTypeConfiguration<ConversationMessage>
{
    public void Configure(EntityTypeBuilder<ConversationMessage> builder)
    {
        builder.ToTable("ConversationMessages");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.Role)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.ContentType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(m => m.Content)
            .IsRequired();

        builder.Property(m => m.ModelName)
            .HasMaxLength(150);

        builder.Property(m => m.ToolCallsJson)
            .HasMaxLength(8000);

        builder.Property(m => m.ToolResultsJson)
            .HasMaxLength(8000);

        builder.Property(m => m.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(m => m.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(m => m.ConversationId)
            .HasDatabaseName("IX_ConversationMessages_ConversationId");

        builder.HasIndex(m => new { m.ConversationId, m.SequenceNumber })
            .IsUnique()
            .HasDatabaseName("IX_ConversationMessages_Conversation_Sequence");

        builder.HasIndex(m => m.Role)
            .HasDatabaseName("IX_ConversationMessages_Role");

        builder.HasIndex(m => m.CreatedAt)
            .HasDatabaseName("IX_ConversationMessages_CreatedAt");

        builder.HasOne(m => m.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(m => m.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(m => !m.IsDeleted);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);
    }
}
