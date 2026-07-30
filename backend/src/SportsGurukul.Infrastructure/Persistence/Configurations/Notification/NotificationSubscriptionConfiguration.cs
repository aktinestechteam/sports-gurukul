using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationSubscriptionConfiguration : IEntityTypeConfiguration<NotificationSubscription>
{
    public void Configure(EntityTypeBuilder<NotificationSubscription> builder)
    {
        builder.ToTable("NotificationSubscriptions");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.EntityType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.EventType)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("IX_NotificationSubscriptions_UserId");

        builder.HasIndex(s => new { s.EntityType, s.EntityId })
            .HasDatabaseName("IX_NotificationSubscriptions_EntityType_EntityId");

        builder.HasIndex(s => new { s.UserId, s.EntityType, s.EntityId, s.EventType })
            .HasDatabaseName("IX_NotificationSubscriptions_UserId_Entity_Event");

        builder.HasOne(s => s.User)
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
