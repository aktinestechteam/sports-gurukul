using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class WorkflowExecutionConfiguration : IEntityTypeConfiguration<WorkflowExecution>
{
    public void Configure(EntityTypeBuilder<WorkflowExecution> builder)
    {
        builder.ToTable("WorkflowExecutions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.TriggerType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.CorrelationId)
            .HasMaxLength(100);

        builder.Property(e => e.TotalCost)
            .HasPrecision(18, 2);

        builder.Property(e => e.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.WorkflowDefinitionId)
            .HasDatabaseName("IX_WorkflowExecutions_WorkflowDefinitionId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_WorkflowExecutions_Status");

        builder.HasIndex(e => e.TriggerType)
            .HasDatabaseName("IX_WorkflowExecutions_TriggerType");

        builder.HasIndex(e => e.CorrelationId)
            .HasDatabaseName("IX_WorkflowExecutions_CorrelationId");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_WorkflowExecutions_CreatedAt");

        builder.HasOne(e => e.WorkflowDefinition)
            .WithMany(w => w.Executions)
            .HasForeignKey(e => e.WorkflowDefinitionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
