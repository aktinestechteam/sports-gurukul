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

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Schema)
            .HasMaxLength(8000);

        builder.Property(e => e.EndpointUrl)
            .HasMaxLength(500);

        builder.Property(e => e.Authentication)
            .HasMaxLength(2000);

        builder.Property(e => e.Parameters)
            .HasMaxLength(4000);

        builder.Property(e => e.ReturnType)
            .HasMaxLength(200);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_ToolDefinitions_Name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_ToolDefinitions_Status");

        builder.HasIndex(e => e.Type)
            .HasDatabaseName("IX_ToolDefinitions_Type");

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
