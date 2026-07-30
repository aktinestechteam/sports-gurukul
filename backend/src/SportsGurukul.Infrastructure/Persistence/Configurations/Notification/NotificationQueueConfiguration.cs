using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationQueueConfiguration : IEntityTypeConfiguration<NotificationQueue>
{
    public void Configure(EntityTypeBuilder<NotificationQueue> builder)
    {
        builder.ToTable("NotificationQueue");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(q => q.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(q => q.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(q => q.LockToken)
            .HasMaxLength(200);

        builder.HasIndex(q => q.NotificationId)
            .IsUnique()
            .HasDatabaseName("IX_NotificationQueue_NotificationId");

        builder.HasIndex(q => q.Status)
            .HasDatabaseName("IX_NotificationQueue_Status");

        builder.HasIndex(q => new { q.Status, q.Priority, q.QueuedAt })
            .HasDatabaseName("IX_NotificationQueue_Status_Priority_QueuedAt");

        builder.HasIndex(q => q.LockExpiresAt)
            .HasDatabaseName("IX_NotificationQueue_LockExpiresAt");

        builder.HasOne(q => q.Notification)
            .WithOne(n => n.QueueEntry)
            .HasForeignKey<NotificationQueue>(q => q.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(q => !q.IsDeleted);
    }
}
