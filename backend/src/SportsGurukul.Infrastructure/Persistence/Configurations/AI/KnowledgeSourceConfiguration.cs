using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.AI;

public class KnowledgeSourceConfiguration : IEntityTypeConfiguration<KnowledgeSource>
{
    public void Configure(EntityTypeBuilder<KnowledgeSource> builder)
    {
        builder.ToTable("KnowledgeSources");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.SourceType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(e => e.SourceUri)
            .HasMaxLength(500);

        builder.Property(e => e.Configuration)
            .HasMaxLength(4000);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.Property(e => e.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(e => e.Name)
            .HasDatabaseName("IX_KnowledgeSources_Name");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_KnowledgeSources_Status");

        builder.HasIndex(e => e.SourceType)
            .HasDatabaseName("IX_KnowledgeSources_SourceType");

        builder.HasIndex(e => e.KnowledgeBaseId)
            .HasDatabaseName("IX_KnowledgeSources_KnowledgeBaseId");

        builder.HasOne(e => e.KnowledgeBase)
            .WithMany(k => k.Sources)
            .HasForeignKey(e => e.KnowledgeBaseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(e => !e.IsDeleted);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);
    }
}
