using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class PromptVersionConfiguration : IEntityTypeConfiguration<PromptVersion>
{
    public void Configure(EntityTypeBuilder<PromptVersion> builder)
    {
        builder.ToTable("PromptVersions");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Content)
            .IsRequired();

        builder.Property(e => e.ChangeNotes)
            .HasMaxLength(2000);

        builder.Property(e => e.Hash)
            .HasMaxLength(64);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.PromptTemplateId)
            .HasDatabaseName("IX_PromptVersions_PromptTemplateId");

        builder.HasIndex(e => new { e.PromptTemplateId, e.VersionNumber })
            .IsUnique()
            .HasDatabaseName("IX_PromptVersions_PromptTemplateId_VersionNumber");

        builder.HasOne(e => e.PromptTemplate)
            .WithMany(p => p.Versions)
            .HasForeignKey(e => e.PromptTemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
