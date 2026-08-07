using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class ToolExecutionConfiguration : IEntityTypeConfiguration<ToolExecution>
{
    public void Configure(EntityTypeBuilder<ToolExecution> builder)
    {
        builder.ToTable("ToolExecutions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.Cost)
            .HasPrecision(18, 2);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.ToolDefinitionId)
            .HasDatabaseName("IX_ToolExecutions_ToolDefinitionId");

        builder.HasIndex(e => e.AgentExecutionId)
            .HasDatabaseName("IX_ToolExecutions_AgentExecutionId");

        builder.HasIndex(e => e.WorkflowExecutionId)
            .HasDatabaseName("IX_ToolExecutions_WorkflowExecutionId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_ToolExecutions_Status");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_ToolExecutions_CreatedAt");

        builder.HasOne(e => e.ToolDefinition)
            .WithMany(t => t.Executions)
            .HasForeignKey(e => e.ToolDefinitionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.AgentExecution)
            .WithMany(a => a.ToolExecutions)
            .HasForeignKey(e => e.AgentExecutionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.WorkflowExecution)
            .WithMany(w => w.ToolExecutions)
            .HasForeignKey(e => e.WorkflowExecutionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
