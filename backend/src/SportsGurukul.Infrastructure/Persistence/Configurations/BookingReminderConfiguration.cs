using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingReminderConfiguration : IEntityTypeConfiguration<BookingReminder>
{
    public void Configure(EntityTypeBuilder<BookingReminder> builder)
    {
        builder.ToTable("BookingReminders");

        builder.HasKey(br => br.Id);

        builder.Property(br => br.Channel)
            .HasMaxLength(50);

        builder.Property(br => br.Notes)
            .HasMaxLength(1000);

        builder.Property(br => br.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(br => br.BookingId)
            .HasDatabaseName("IX_BookingReminders_BookingId");

        builder.HasIndex(br => br.ScheduledAt)
            .HasDatabaseName("IX_BookingReminders_ScheduledAt");

        builder.HasIndex(br => br.IsSent)
            .HasDatabaseName("IX_BookingReminders_IsSent");

        builder.HasIndex(br => new { br.BookingId, br.IsSent })
            .HasDatabaseName("IX_BookingReminders_BookingId_IsSent");

        builder.HasOne(br => br.Booking)
            .WithMany(b => b.Reminders)
            .HasForeignKey(br => br.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(br => !br.IsDeleted);

        builder.Ignore(br => br.CreatedBy);
        builder.Ignore(br => br.UpdatedBy);
    }
}
