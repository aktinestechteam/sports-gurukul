using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationAuditConfiguration : IEntityTypeConfiguration<NotificationAudit>
{
    public void Configure(EntityTypeBuilder<NotificationAudit> builder)
    {
        builder.ToTable("NotificationAudits");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(a => a.OldValue)
            .HasMaxLength(8000);

        builder.Property(a => a.NewValue)
            .HasMaxLength(8000);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(50);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.HasIndex(a => new { a.EntityType, a.EntityId })
            .HasDatabaseName("IX_NotificationAudits_EntityType_EntityId");

        builder.HasIndex(a => a.Action)
            .HasDatabaseName("IX_NotificationAudits_Action");

        builder.HasIndex(a => a.ChangedAt)
            .HasDatabaseName("IX_NotificationAudits_ChangedAt");

        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}
