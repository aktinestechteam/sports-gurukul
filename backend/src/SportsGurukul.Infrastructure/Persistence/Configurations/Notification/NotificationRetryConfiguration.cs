using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationRetryConfiguration : IEntityTypeConfiguration<NotificationRetry>
{
    public void Configure(EntityTypeBuilder<NotificationRetry> builder)
    {
        builder.ToTable("NotificationRetries");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(r => r.FailureReason)
            .HasMaxLength(2000);

        builder.HasIndex(r => r.DeliveryId)
            .HasDatabaseName("IX_NotificationRetries_DeliveryId");

        builder.HasIndex(r => new { r.DeliveryId, r.AttemptNumber })
            .IsUnique()
            .HasDatabaseName("IX_NotificationRetries_DeliveryId_AttemptNumber");

        builder.HasOne(r => r.Delivery)
            .WithMany(d => d.Retries)
            .HasForeignKey(r => r.DeliveryId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(r => !r.IsDeleted);
    }
}
