using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class TemplateVersionConfiguration : IEntityTypeConfiguration<TemplateVersion>
{
    public void Configure(EntityTypeBuilder<TemplateVersion> builder)
    {
        builder.ToTable("TemplateVersions");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.SubjectTemplate)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(v => v.BodyTemplate)
            .IsRequired();

        builder.Property(v => v.ChangeNotes)
            .HasMaxLength(2000);

        builder.HasIndex(v => new { v.TemplateId, v.VersionNumber })
            .IsUnique()
            .HasDatabaseName("IX_TemplateVersions_TemplateId_VersionNumber");

        builder.HasOne(v => v.Template)
            .WithMany(t => t.Versions)
            .HasForeignKey(v => v.TemplateId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(v => !v.IsDeleted);
    }
}
