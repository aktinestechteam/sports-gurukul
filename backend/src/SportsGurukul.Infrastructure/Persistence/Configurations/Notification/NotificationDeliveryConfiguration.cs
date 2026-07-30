using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationDeliveryConfiguration : IEntityTypeConfiguration<NotificationDelivery>
{
    public void Configure(EntityTypeBuilder<NotificationDelivery> builder)
    {
        builder.ToTable("NotificationDeliveries");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(d => d.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(d => d.FailureReason)
            .HasMaxLength(2000);

        builder.Property(d => d.ProviderMessageId)
            .HasMaxLength(500);

        builder.Property(d => d.ProviderResponse)
            .HasMaxLength(4000);

        builder.HasIndex(d => d.NotificationId)
            .HasDatabaseName("IX_NotificationDeliveries_NotificationId");

        builder.HasIndex(d => d.Status)
            .HasDatabaseName("IX_NotificationDeliveries_Status");

        builder.HasIndex(d => d.ProviderMessageId)
            .HasDatabaseName("IX_NotificationDeliveries_ProviderMessageId");

        builder.HasIndex(d => new { d.Status, d.AttemptCount })
            .HasDatabaseName("IX_NotificationDeliveries_Status_AttemptCount");

        builder.HasOne(d => d.Notification)
            .WithMany(n => n.Deliveries)
            .HasForeignKey(d => d.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Provider)
            .WithMany(p => p.Deliveries)
            .HasForeignKey(d => d.ProviderId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
