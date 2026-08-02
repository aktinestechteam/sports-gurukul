using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class ToolExecutionConfiguration : IEntityTypeConfiguration<ToolExecution>
{
    public void Configure(EntityTypeBuilder<ToolExecution> builder)
    {
        builder.ToTable("ToolExecutions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Input)
            .HasMaxLength(8000);

        builder.Property(e => e.Output)
            .HasMaxLength(8000);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.Cost)
            .HasPrecision(18, 8);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.ToolDefinitionId)
            .HasDatabaseName("IX_ToolExecutions_ToolDefinitionId");

        builder.HasIndex(e => e.ConversationId)
            .HasDatabaseName("IX_ToolExecutions_ConversationId");

        builder.HasOne(e => e.ToolDefinition)
            .WithMany(t => t.Executions)
            .HasForeignKey(e => e.ToolDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Conversation)
            .WithMany()
            .HasForeignKey(e => e.ConversationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
