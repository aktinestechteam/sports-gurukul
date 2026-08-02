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

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Configuration)
            .HasMaxLength(8000);

        builder.Property(e => e.Tools)
            .HasMaxLength(4000);

        builder.Property(e => e.Rules)
            .HasMaxLength(8000);

        builder.Property(e => e.Constraints)
            .HasMaxLength(4000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_AgentDefinitions_Name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_AgentDefinitions_Status");

        builder.HasIndex(e => e.AssistantId)
            .HasDatabaseName("IX_AgentDefinitions_AssistantId");

        builder.HasOne(e => e.Assistant)
            .WithMany(a => a.AgentDefinitions)
            .HasForeignKey(e => e.AssistantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
