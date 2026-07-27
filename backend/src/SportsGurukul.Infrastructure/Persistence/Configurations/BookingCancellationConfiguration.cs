using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingCancellationConfiguration : IEntityTypeConfiguration<BookingCancellation>
{
    public void Configure(EntityTypeBuilder<BookingCancellation> builder)
    {
        builder.ToTable("BookingCancellations");

        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(bc => bc.RefundAmount)
            .HasPrecision(18, 2);

        builder.Property(bc => bc.Notes)
            .HasMaxLength(1000);

        builder.Property(bc => bc.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(bc => bc.BookingId)
            .HasDatabaseName("IX_BookingCancellations_BookingId");

        builder.HasIndex(bc => bc.CancelledByUserId)
            .HasDatabaseName("IX_BookingCancellations_CancelledByUserId");

        builder.HasIndex(bc => bc.CancelledOn)
            .HasDatabaseName("IX_BookingCancellations_CancelledOn");

        builder.HasIndex(bc => new { bc.BookingId, bc.CancelledOn })
            .HasDatabaseName("IX_BookingCancellations_BookingId_CancelledOn");

        builder.HasOne(bc => bc.Booking)
            .WithMany(b => b.Cancellations)
            .HasForeignKey(bc => bc.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(bc => !bc.IsDeleted);

        builder.Ignore(bc => bc.CreatedBy);
        builder.Ignore(bc => bc.UpdatedBy);
    }
}
