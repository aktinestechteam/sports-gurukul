using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingRecurrenceConfiguration : IEntityTypeConfiguration<BookingRecurrence>
{
    public void Configure(EntityTypeBuilder<BookingRecurrence> builder)
    {
        builder.ToTable("BookingRecurrences");

        builder.HasKey(br => br.Id);

        builder.Property(br => br.RecurrenceType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(br => br.RRule)
            .HasMaxLength(500);

        builder.Property(br => br.Exceptions)
            .HasMaxLength(2000);

        builder.Property(br => br.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(br => br.BookingId)
            .HasDatabaseName("IX_BookingRecurrences_BookingId");

        builder.HasIndex(br => br.RecurrenceType)
            .HasDatabaseName("IX_BookingRecurrences_RecurrenceType");

        builder.HasIndex(br => br.IsActive)
            .HasDatabaseName("IX_BookingRecurrences_IsActive");

        builder.HasIndex(br => new { br.BookingId, br.IsActive })
            .HasDatabaseName("IX_BookingRecurrences_BookingId_IsActive");

        builder.HasOne(br => br.Booking)
            .WithMany(b => b.Recurrences)
            .HasForeignKey(br => br.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(br => !br.IsDeleted);

        builder.Ignore(br => br.CreatedBy);
        builder.Ignore(br => br.UpdatedBy);

        builder.HasData(
            new BookingRecurrence
            {
                Id = Guid.Parse("b5000000-0000-0000-0000-000000000001"),
                BookingId = Guid.Parse("b1000000-0000-0000-0000-000000000001"),
                RecurrenceType = RecurrenceType.Weekly,
                EndDate = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc),
                OccurrenceCount = 24,
                IsActive = true,
                IsDeleted = false
            }
        );
    }
}
