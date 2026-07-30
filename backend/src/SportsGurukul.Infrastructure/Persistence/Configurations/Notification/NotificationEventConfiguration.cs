using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationEventConfiguration : IEntityTypeConfiguration<NotificationEvent>
{
    public void Configure(EntityTypeBuilder<NotificationEvent> builder)
    {
        builder.ToTable("NotificationEvents");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Payload)
            .HasMaxLength(8000);

        builder.Property(e => e.Status)
            .HasMaxLength(50)
            .HasDefaultValue("Pending");

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000);

        builder.HasIndex(e => e.EventType)
            .HasDatabaseName("IX_NotificationEvents_EventType");

        builder.HasIndex(e => new { e.EntityType, e.EntityId })
            .HasDatabaseName("IX_NotificationEvents_EntityType_EntityId");

        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_NotificationEvents_Status");

        builder.HasIndex(e => e.OccurredAt)
            .HasDatabaseName("IX_NotificationEvents_OccurredAt");

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
