using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class PromptVersionConfiguration : IEntityTypeConfiguration<PromptVersion>
{
    public void Configure(EntityTypeBuilder<PromptVersion> builder)
    {
        builder.ToTable("PromptVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.Content)
            .IsRequired();

        builder.Property(v => v.ChangeSummary)
            .HasMaxLength(2000);

        builder.Property(v => v.Notes)
            .HasMaxLength(2000);

        builder.Property(v => v.EvaluationJson)
            .HasMaxLength(8000);

        builder.Property(v => v.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(v => v.PromptTemplateId)
            .HasDatabaseName("IX_PromptVersions_PromptTemplateId");

        builder.HasIndex(v => new { v.PromptTemplateId, v.VersionNumber })
            .IsUnique()
            .HasDatabaseName("IX_PromptVersions_Template_Version");

        builder.HasIndex(v => v.IsActive)
            .HasDatabaseName("IX_PromptVersions_IsActive");

        builder.HasOne(v => v.PromptTemplate)
            .WithMany(t => t.Versions)
            .HasForeignKey(v => v.PromptTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(v => !v.IsDeleted);

        builder.Ignore(v => v.CreatedBy);
        builder.Ignore(v => v.UpdatedBy);
    }
}
