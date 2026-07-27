using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingWaitlistConfiguration : IEntityTypeConfiguration<BookingWaitlist>
{
    public void Configure(EntityTypeBuilder<BookingWaitlist> builder)
    {
        builder.ToTable("BookingWaitlists");

        builder.HasKey(bw => bw.Id);

        builder.Property(bw => bw.Notes)
            .HasMaxLength(1000);

        builder.Property(bw => bw.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(bw => bw.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(bw => bw.BookingId)
            .HasDatabaseName("IX_BookingWaitlists_BookingId");

        builder.HasIndex(bw => bw.WaitlistUserId)
            .HasDatabaseName("IX_BookingWaitlists_WaitlistUserId");

        builder.HasIndex(bw => bw.Status)
            .HasDatabaseName("IX_BookingWaitlists_Status");

        builder.HasIndex(bw => bw.Priority)
            .HasDatabaseName("IX_BookingWaitlists_Priority");

        builder.HasIndex(bw => new { bw.BookingId, bw.Priority })
            .HasDatabaseName("IX_BookingWaitlists_BookingId_Priority");

        builder.HasIndex(bw => new { bw.BookingId, bw.WaitlistUserId })
            .IsUnique()
            .HasDatabaseName("IX_BookingWaitlists_BookingId_UserId");

        builder.HasOne(bw => bw.Booking)
            .WithMany(b => b.WaitlistEntries)
            .HasForeignKey(bw => bw.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(bw => !bw.IsDeleted);

        builder.Ignore(bw => bw.CreatedBy);
        builder.Ignore(bw => bw.UpdatedBy);
    }
}
