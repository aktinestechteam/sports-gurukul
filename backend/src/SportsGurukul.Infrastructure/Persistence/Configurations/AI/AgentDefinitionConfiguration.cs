using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class AgentDefinitionConfiguration : IEntityTypeConfiguration<AgentDefinition>
{
    public void Configure(EntityTypeBuilder<AgentDefinition> builder)
    {
        builder.ToTable("AgentDefinitions");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(a => a.Description)
            .HasMaxLength(1000);

        builder.Property(a => a.AgentType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.ToolsJson)
            .HasMaxLength(8000);

        builder.Property(a => a.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(a => a.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(a => a.WorkflowId)
            .HasDatabaseName("IX_AgentDefinitions_WorkflowId");

        builder.HasIndex(a => a.ModelId)
            .HasDatabaseName("IX_AgentDefinitions_ModelId");

        builder.HasIndex(a => new { a.Name, a.WorkflowId })
            .IsUnique()
            .HasDatabaseName("IX_AgentDefinitions_WorkflowId_Name");

        builder.HasIndex(a => a.AgentType)
            .HasDatabaseName("IX_AgentDefinitions_AgentType");

        builder.HasIndex(a => a.IsActive)
            .HasDatabaseName("IX_AgentDefinitions_IsActive");

        builder.HasOne(a => a.Workflow)
            .WithMany(w => w.Agents)
            .HasForeignKey(a => a.WorkflowId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Model)
            .WithMany()
            .HasForeignKey(a => a.ModelId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(a => !a.IsDeleted);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
