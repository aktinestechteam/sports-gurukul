using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class PromptTemplateConfiguration : IEntityTypeConfiguration<PromptTemplate>
{
    public void Configure(EntityTypeBuilder<PromptTemplate> builder)
    {
        builder.ToTable("PromptTemplates");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(1000);

        builder.Property(t => t.PromptType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(t => t.TemplateText)
            .IsRequired();

        builder.Property(t => t.InputSchemaJson)
            .HasMaxLength(8000);

        builder.Property(t => t.OutputSchemaJson)
            .HasMaxLength(8000);

        builder.Property(t => t.VariablesJson)
            .HasMaxLength(8000);

        builder.Property(t => t.MetadataJson)
            .HasMaxLength(8000);

        builder.Property(t => t.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(t => t.AssistantId)
            .HasDatabaseName("IX_PromptTemplates_AssistantId");

        builder.HasIndex(t => new { t.AssistantId, t.Name })
            .IsUnique()
            .HasDatabaseName("IX_PromptTemplates_AssistantId_Name");

        builder.HasIndex(t => t.PromptType)
            .HasDatabaseName("IX_PromptTemplates_PromptType");

        builder.HasIndex(t => t.IsActive)
            .HasDatabaseName("IX_PromptTemplates_IsActive");

        builder.HasOne(t => t.Assistant)
            .WithMany(a => a.PromptTemplates)
            .HasForeignKey(t => t.AssistantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(t => !t.IsDeleted);

        builder.Ignore(t => t.CreatedBy);
        builder.Ignore(t => t.UpdatedBy);
    }
}
