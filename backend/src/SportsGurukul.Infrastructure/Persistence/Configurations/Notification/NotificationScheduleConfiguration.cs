using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationScheduleConfiguration : IEntityTypeConfiguration<NotificationSchedule>
{
    public void Configure(EntityTypeBuilder<NotificationSchedule> builder)
    {
        builder.ToTable("NotificationSchedules");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.TimeZone)
            .HasMaxLength(100)
            .HasDefaultValue("UTC");

        builder.Property(s => s.RecurrenceRule)
            .HasMaxLength(500);

        builder.Property(s => s.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(s => s.NotificationId)
            .IsUnique()
            .HasDatabaseName("IX_NotificationSchedules_NotificationId");

        builder.HasIndex(s => s.ScheduledAtUtc)
            .HasDatabaseName("IX_NotificationSchedules_ScheduledAtUtc");

        builder.HasIndex(s => s.Status)
            .HasDatabaseName("IX_NotificationSchedules_Status");

        builder.HasOne(s => s.Notification)
            .WithOne(n => n.Schedule)
            .HasForeignKey<NotificationSchedule>(s => s.NotificationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(s => !s.IsDeleted);
    }
}
