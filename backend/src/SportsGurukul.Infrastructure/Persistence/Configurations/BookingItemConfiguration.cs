using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingItemConfiguration : IEntityTypeConfiguration<BookingItem>
{
    public void Configure(EntityTypeBuilder<BookingItem> builder)
    {
        builder.ToTable("BookingItems");

        builder.HasKey(bi => bi.Id);

        builder.Property(bi => bi.ItemName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(bi => bi.ItemDescription)
            .HasMaxLength(1000);

        builder.Property(bi => bi.Unit)
            .HasMaxLength(50);

        builder.Property(bi => bi.UnitPrice)
            .HasPrecision(18, 2);

        builder.Property(bi => bi.TotalPrice)
            .HasPrecision(18, 2);

        builder.Property(bi => bi.Notes)
            .HasMaxLength(1000);

        builder.Property(bi => bi.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(bi => bi.BookingId)
            .HasDatabaseName("IX_BookingItems_BookingId");

        builder.HasIndex(bi => new { bi.BookingId, bi.ItemName })
            .HasDatabaseName("IX_BookingItems_BookingId_ItemName");

        builder.HasOne(bi => bi.Booking)
            .WithMany(b => b.Items)
            .HasForeignKey(bi => bi.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(bi => !bi.IsDeleted);

        builder.Ignore(bi => bi.CreatedBy);
        builder.Ignore(bi => bi.UpdatedBy);
    }
}
