using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationChannelConfiguration : IEntityTypeConfiguration<NotificationChannel>
{
    public void Configure(EntityTypeBuilder<NotificationChannel> builder)
    {
        builder.ToTable("NotificationChannels");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(c => c.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.SortOrder)
            .HasDefaultValue(0);

        builder.HasIndex(c => c.Code)
            .IsUnique()
            .HasDatabaseName("IX_NotificationChannels_Code");

        builder.HasIndex(c => c.ChannelType)
            .HasDatabaseName("IX_NotificationChannels_ChannelType");

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasData(
            new NotificationChannel { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Email", Code = "email", ChannelType = NotificationChannelType.Email, IsActive = true, SortOrder = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationChannel { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "SMS", Code = "sms", ChannelType = NotificationChannelType.SMS, IsActive = true, SortOrder = 2, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationChannel { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "WhatsApp", Code = "whatsapp", ChannelType = NotificationChannelType.WhatsApp, IsActive = true, SortOrder = 3, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationChannel { Id = Guid.Parse("00000000-0000-0000-0000-000000000004"), Name = "Push Notification", Code = "push", ChannelType = NotificationChannelType.PushNotification, IsActive = true, SortOrder = 4, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationChannel { Id = Guid.Parse("00000000-0000-0000-0000-000000000005"), Name = "In-App Notification", Code = "inapp", ChannelType = NotificationChannelType.InAppNotification, IsActive = true, SortOrder = 5, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationChannel { Id = Guid.Parse("00000000-0000-0000-0000-000000000006"), Name = "Webhook", Code = "webhook", ChannelType = NotificationChannelType.Webhook, IsActive = true, SortOrder = 6, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
