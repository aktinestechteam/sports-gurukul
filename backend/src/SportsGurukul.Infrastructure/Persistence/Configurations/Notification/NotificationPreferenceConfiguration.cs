using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Infrastructure.Persistence.Configurations.Notification;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreferences");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ChannelType)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.Property(p => p.QuietHoursStart)
            .HasColumnType("time");

        builder.Property(p => p.QuietHoursEnd)
            .HasColumnType("time");

        builder.HasIndex(p => p.UserId)
            .HasDatabaseName("IX_NotificationPreferences_UserId");

        builder.HasIndex(p => new { p.UserId, p.ChannelType })
            .IsUnique()
            .HasDatabaseName("IX_NotificationPreferences_UserId_ChannelType");

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}
