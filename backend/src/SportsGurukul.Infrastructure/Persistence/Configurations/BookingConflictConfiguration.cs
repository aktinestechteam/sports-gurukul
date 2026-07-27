using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingConflictConfiguration : IEntityTypeConfiguration<BookingConflict>
{
    public void Configure(EntityTypeBuilder<BookingConflict> builder)
    {
        builder.ToTable("BookingConflicts");

        builder.HasKey(bc => bc.Id);

        builder.Property(bc => bc.ConflictType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(bc => bc.Description)
            .HasMaxLength(1000);

        builder.Property(bc => bc.ResolutionNotes)
            .HasMaxLength(1000);

        builder.Property(bc => bc.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(bc => bc.BookingId)
            .HasDatabaseName("IX_BookingConflicts_BookingId");

        builder.HasIndex(bc => bc.ConflictingBookingId)
            .HasDatabaseName("IX_BookingConflicts_ConflictingBookingId");

        builder.HasIndex(bc => bc.ConflictType)
            .HasDatabaseName("IX_BookingConflicts_ConflictType");

        builder.HasIndex(bc => bc.IsResolved)
            .HasDatabaseName("IX_BookingConflicts_IsResolved");

        builder.HasIndex(bc => new { bc.BookingId, bc.IsResolved })
            .HasDatabaseName("IX_BookingConflicts_BookingId_IsResolved");

        builder.HasIndex(bc => new { bc.ConflictType, bc.IsResolved })
            .HasDatabaseName("IX_BookingConflicts_Type_IsResolved");

        builder.HasOne(bc => bc.Booking)
            .WithMany(b => b.Conflicts)
            .HasForeignKey(bc => bc.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(bc => !bc.IsDeleted);

        builder.Ignore(bc => bc.CreatedBy);
        builder.Ignore(bc => bc.UpdatedBy);
    }
}
