using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationRecipientConfiguration : IEntityTypeConfiguration<NotificationRecipient>
{
    public void Configure(EntityTypeBuilder<NotificationRecipient> builder)
    {
        builder.ToTable("NotificationRecipients");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(r => r.DestinationAddress)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(r => r.RecipientName)
            .HasMaxLength(200);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.FailureReason)
            .HasMaxLength(2000);

        builder.HasIndex(r => r.NotificationId)
            .HasDatabaseName("IX_NotificationRecipients_NotificationId");

        builder.HasIndex(r => r.UserId)
            .HasDatabaseName("IX_NotificationRecipients_UserId");

        builder.HasIndex(r => r.Status)
            .HasDatabaseName("IX_NotificationRecipients_Status");

        builder.HasIndex(r => new { r.NotificationId, r.UserId })
            .HasDatabaseName("IX_NotificationRecipients_NotificationId_UserId");

        builder.HasOne(r => r.Notification)
            .WithMany(n => n.Recipients)
            .HasForeignKey(r => r.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
