using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationProviderConfiguration : IEntityTypeConfiguration<NotificationProvider>
{
    public void Configure(EntityTypeBuilder<NotificationProvider> builder)
    {
        builder.ToTable("NotificationProviders");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(p => p.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.Configuration)
            .HasMaxLength(4000);

        builder.Property(p => p.Priority)
            .HasDefaultValue(0);

        builder.HasIndex(p => new { p.ChannelId, p.IsDefault })
            .HasDatabaseName("IX_NotificationProviders_ChannelId_IsDefault");

        builder.HasIndex(p => new { p.ChannelId, p.Priority })
            .HasDatabaseName("IX_NotificationProviders_ChannelId_Priority");

        builder.HasOne(p => p.Channel)
            .WithMany(c => c.Providers)
            .HasForeignKey(p => p.ChannelId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasQueryFilter(p => !p.IsDeleted);

        builder.HasData(
            new NotificationProvider { Id = Guid.Parse("00000000-0000-0000-0000-000000000010"), Name = "SMTP Email Provider", ChannelType = NotificationChannelType.Email, ChannelId = Guid.Parse("00000000-0000-0000-0000-000000000001"), IsActive = true, IsDefault = true, Priority = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationProvider { Id = Guid.Parse("00000000-0000-0000-0000-000000000011"), Name = "Twilio SMS Provider", ChannelType = NotificationChannelType.SMS, ChannelId = Guid.Parse("00000000-0000-0000-0000-000000000002"), IsActive = true, IsDefault = true, Priority = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationProvider { Id = Guid.Parse("00000000-0000-0000-0000-000000000012"), Name = "Twilio WhatsApp Provider", ChannelType = NotificationChannelType.WhatsApp, ChannelId = Guid.Parse("00000000-0000-0000-0000-000000000003"), IsActive = true, IsDefault = true, Priority = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationProvider { Id = Guid.Parse("00000000-0000-0000-0000-000000000013"), Name = "Firebase Push Provider", ChannelType = NotificationChannelType.PushNotification, ChannelId = Guid.Parse("00000000-0000-0000-0000-000000000004"), IsActive = true, IsDefault = true, Priority = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationProvider { Id = Guid.Parse("00000000-0000-0000-0000-000000000014"), Name = "SignalR In-App Provider", ChannelType = NotificationChannelType.InAppNotification, ChannelId = Guid.Parse("00000000-0000-0000-0000-000000000005"), IsActive = true, IsDefault = true, Priority = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new NotificationProvider { Id = Guid.Parse("00000000-0000-0000-0000-000000000015"), Name = "Webhook HTTP Provider", ChannelType = NotificationChannelType.Webhook, ChannelId = Guid.Parse("00000000-0000-0000-0000-000000000006"), IsActive = true, IsDefault = true, Priority = 1, CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}
