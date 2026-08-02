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

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Role)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.PromptTokens)
            .HasMaxLength(4000);

        builder.Property(e => e.CompletionTokens)
            .HasMaxLength(4000);

        builder.Property(e => e.ToolCalls)
            .HasMaxLength(8000);

        builder.Property(e => e.ToolResults)
            .HasMaxLength(8000);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.Cost)
            .HasPrecision(18, 8);

        builder.Property(e => e.Metadata)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.ConversationId)
            .HasDatabaseName("IX_ConversationMessages_ConversationId");

        builder.HasIndex(e => e.Role)
            .HasDatabaseName("IX_ConversationMessages_Role");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_ConversationMessages_Status");

        builder.HasOne(e => e.Conversation)
            .WithMany(c => c.Messages)
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
