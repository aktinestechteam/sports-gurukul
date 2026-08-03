using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class ToolDefinitionConfiguration : IEntityTypeConfiguration<ToolDefinition>
{
    public void Configure(EntityTypeBuilder<ToolDefinition> builder)
    {
        builder.ToTable("ToolDefinitions");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.ToolType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.Endpoint)
            .HasMaxLength(2000);

        builder.Property(t => t.HttpMethod)
            .HasMaxLength(10);

        builder.Property(t => t.OutputSchemaJson)
            .HasMaxLength(8000);

        builder.Property(t => t.RetryPolicyJson)
            .HasMaxLength(4000);

        builder.Property(t => t.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(t => t.AgentId)
            .HasDatabaseName("IX_ToolDefinitions_AgentId");

        builder.HasIndex(t => new { t.AgentId, t.Name })
            .IsUnique()
            .HasDatabaseName("IX_ToolDefinitions_AgentId_Name");

        builder.HasIndex(t => t.ToolType)
            .HasDatabaseName("IX_ToolDefinitions_ToolType");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_ToolDefinitions_IsActive");

        builder.HasOne(t => t.Agent)
            .WithMany(a => a.Tools)
            .HasForeignKey(t => t.AgentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.Ignore(t => t.CreatedBy);
        builder.Ignore(t => t.UpdatedBy);
    }
}
