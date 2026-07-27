using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingRescheduleConfiguration : IEntityTypeConfiguration<BookingReschedule>
{
    public void Configure(EntityTypeBuilder<BookingReschedule> builder)
    {
        builder.ToTable("BookingReschedules");

        builder.HasKey(br => br.Id);

        builder.Property(br => br.Reason)
            .HasMaxLength(1000);

        builder.Property(br => br.Notes)
            .HasMaxLength(1000);

        builder.Property(br => br.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(br => br.BookingId)
            .HasDatabaseName("IX_BookingReschedules_BookingId");

        builder.HasIndex(br => br.RequestedById)
            .HasDatabaseName("IX_BookingReschedules_RequestedById");

        builder.HasIndex(br => new { br.BookingId, br.IsApproved })
            .HasDatabaseName("IX_BookingReschedules_BookingId_IsApproved");

        builder.HasOne(br => br.Booking)
            .WithMany(b => b.Reschedules)
            .HasForeignKey(br => br.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(br => !br.IsDeleted);

        builder.Ignore(br => br.CreatedBy);
        builder.Ignore(br => br.UpdatedBy);
    }
}
