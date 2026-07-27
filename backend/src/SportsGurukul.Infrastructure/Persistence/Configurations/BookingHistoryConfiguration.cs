using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingHistoryConfiguration : IEntityTypeConfiguration<BookingHistory>
{
    public void Configure(EntityTypeBuilder<BookingHistory> builder)
    {
        builder.ToTable("BookingHistories");

        builder.HasKey(bh => bh.Id);

        builder.Property(bh => bh.Action)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(bh => bh.PreviousValue)
            .HasMaxLength(2000);

        builder.Property(bh => bh.NewValue)
            .HasMaxLength(2000);

        builder.Property(bh => bh.PerformedBy)
            .HasMaxLength(200);

        builder.Property(bh => bh.Notes)
            .HasMaxLength(1000);

        builder.Property(bh => bh.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(bh => bh.BookingId)
            .HasDatabaseName("IX_BookingHistories_BookingId");

        builder.HasIndex(bh => bh.Action)
            .HasDatabaseName("IX_BookingHistories_Action");

        builder.HasIndex(bh => bh.PerformedOn)
            .HasDatabaseName("IX_BookingHistories_PerformedOn");

        builder.HasIndex(bh => new { bh.BookingId, bh.PerformedOn })
            .HasDatabaseName("IX_BookingHistories_BookingId_PerformedOn");

        builder.HasOne(bh => bh.Booking)
            .WithMany(b => b.History)
            .HasForeignKey(bh => bh.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(bh => !bh.IsDeleted);

        builder.Ignore(bh => bh.CreatedBy);
        builder.Ignore(bh => bh.UpdatedBy);
    }
}
