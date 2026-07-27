using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingScheduleConfiguration : IEntityTypeConfiguration<BookingSchedule>
{
    public void Configure(EntityTypeBuilder<BookingSchedule> builder)
    {
        builder.ToTable("BookingSchedules");

        builder.HasKey(bs => bs.Id);

        builder.Property(bs => bs.CancellationReason)
            .HasMaxLength(500);

        builder.Property(bs => bs.Notes)
            .HasMaxLength(1000);

        builder.Property(bs => bs.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(bs => bs.BookingId)
            .HasDatabaseName("IX_BookingSchedules_BookingId");

        builder.HasIndex(bs => bs.ScheduledDate)
            .HasDatabaseName("IX_BookingSchedules_ScheduledDate");

        builder.HasIndex(bs => new { bs.BookingId, bs.ScheduledDate })
            .HasDatabaseName("IX_BookingSchedules_BookingId_ScheduledDate");

        builder.HasOne(bs => bs.Booking)
            .WithMany(b => b.Schedules)
            .HasForeignKey(bs => bs.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(bs => !bs.IsDeleted);

        builder.Ignore(bs => bs.CreatedBy);
        builder.Ignore(bs => bs.UpdatedBy);
    }
}
