using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationConfiguration : IEntityTypeConfiguration<Domain.Entities.Notification.Notification>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Notification.Notification> builder)
    {
        builder.ToTable("Notifications");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Priority)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(n => n.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(n => n.Subject)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(n => n.Body)
            .IsRequired();

        builder.Property(n => n.SenderId)
            .HasMaxLength(100);

        builder.Property(n => n.FailureReason)
            .HasMaxLength(2000);

        builder.Property(n => n.ErrorCode)
            .HasMaxLength(100);

        builder.Property(n => n.ExternalId)
            .HasMaxLength(200);

        builder.Property(n => n.Metadata)
            .HasMaxLength(4000);

        builder.Property(n => n.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(n => n.Status)
            .HasDatabaseName("IX_Notifications_Status");

        builder.HasIndex(n => n.Priority)
            .HasDatabaseName("IX_Notifications_Priority");

        builder.HasIndex(n => n.ScheduledAt)
            .HasDatabaseName("IX_Notifications_ScheduledAt");

        builder.HasIndex(n => n.BatchId)
            .HasDatabaseName("IX_Notifications_BatchId");

        builder.HasIndex(n => n.CampaignId)
            .HasDatabaseName("IX_Notifications_CampaignId");

        builder.HasIndex(n => n.ExternalId)
            .HasDatabaseName("IX_Notifications_ExternalId");

        builder.HasIndex(n => new { n.Status, n.Priority })
            .HasDatabaseName("IX_Notifications_Status_Priority");

        builder.HasOne(n => n.Template)
            .WithMany(t => t.Notifications)
            .HasForeignKey(n => n.TemplateId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(n => n.Channel)
            .WithMany(c => c.Notifications)
            .HasForeignKey(n => n.ChannelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(n => n.Provider)
            .WithMany(p => p.Notifications)
            .HasForeignKey(n => n.ProviderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(n => n.Batch)
            .WithMany(b => b.Notifications)
            .HasForeignKey(n => n.BatchId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(n => n.Campaign)
            .WithMany(c => c.Notifications)
            .HasForeignKey(n => n.CampaignId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(n => n.Schedule)
            .WithOne(s => s.Notification)
            .HasForeignKey<Domain.Entities.Notification.NotificationSchedule>(s => s.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(n => n.QueueEntry)
            .WithOne(q => q.Notification)
            .HasForeignKey<Domain.Entities.Notification.NotificationQueue>(q => q.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(n => !n.IsDeleted);

        builder.Ignore(n => n.CreatedBy);
        builder.Ignore(n => n.UpdatedBy);
    }
}
