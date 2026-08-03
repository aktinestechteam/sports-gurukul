using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class WorkflowDefinitionConfiguration : IEntityTypeConfiguration<WorkflowDefinition>
{
    public void Configure(EntityTypeBuilder<WorkflowDefinition> builder)
    {
        builder.ToTable("WorkflowDefinitions");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(w => w.Description)
            .HasMaxLength(1000);

        builder.Property(w => w.WorkflowType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(w => w.EntryNode)
            .HasMaxLength(150);

        builder.Property(w => w.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(w => w.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(w => w.Name)
            .IsUnique()
            .HasDatabaseName("IX_WorkflowDefinitions_Name");

        builder.HasIndex(w => w.WorkflowType)
            .HasDatabaseName("IX_WorkflowDefinitions_WorkflowType");

        builder.HasIndex(w => new { w.IsActive, w.IsPublished })
            .HasDatabaseName("IX_WorkflowDefinitions_IsActive_IsPublished");

        builder.HasQueryFilter(w => !w.IsDeleted);

        builder.Ignore(w => w.CreatedBy);
        builder.Ignore(w => w.UpdatedBy);
    }
}
