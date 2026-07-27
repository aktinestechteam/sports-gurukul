using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingAttachmentConfiguration : IEntityTypeConfiguration<BookingAttachment>
{
    public void Configure(EntityTypeBuilder<BookingAttachment> builder)
    {
        builder.ToTable("BookingAttachments");

        builder.HasKey(ba => ba.Id);

        builder.Property(ba => ba.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(ba => ba.FileType)
            .HasMaxLength(50);

        builder.Property(ba => ba.FileUrl)
            .HasMaxLength(2000)
            .IsRequired();

        builder.Property(ba => ba.Description)
            .HasMaxLength(1000);

        builder.Property(ba => ba.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(ba => ba.BookingId)
            .HasDatabaseName("IX_BookingAttachments_BookingId");

        builder.HasIndex(ba => ba.FileName)
            .HasDatabaseName("IX_BookingAttachments_FileName");

        builder.HasIndex(ba => new { ba.BookingId, ba.FileName })
            .HasDatabaseName("IX_BookingAttachments_BookingId_FileName");

        builder.HasOne(ba => ba.Booking)
            .WithMany(b => b.Attachments)
            .HasForeignKey(ba => ba.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(ba => !ba.IsDeleted);

        builder.Ignore(ba => ba.CreatedBy);
        builder.Ignore(ba => ba.UpdatedBy);
    }
}
