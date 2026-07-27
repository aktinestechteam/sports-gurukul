using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class BookingParticipantConfiguration : IEntityTypeConfiguration<BookingParticipant>
{
    public void Configure(EntityTypeBuilder<BookingParticipant> builder)
    {
        builder.ToTable("BookingParticipants");

        builder.HasKey(bp => bp.Id);

        builder.Property(bp => bp.ParticipantName)
            .HasMaxLength(200);

        builder.Property(bp => bp.Role)
            .HasMaxLength(50);

        builder.Property(bp => bp.Notes)
            .HasMaxLength(1000);

        builder.Property(bp => bp.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(bp => bp.BookingId)
            .HasDatabaseName("IX_BookingParticipants_BookingId");

        builder.HasIndex(bp => bp.ParticipantId)
            .HasDatabaseName("IX_BookingParticipants_ParticipantId");

        builder.HasIndex(bp => new { bp.BookingId, bp.ParticipantId })
            .IsUnique()
            .HasDatabaseName("IX_BookingParticipants_BookingId_ParticipantId");

        builder.HasOne(bp => bp.Booking)
            .WithMany(b => b.Participants)
            .HasForeignKey(bp => bp.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(bp => !bp.IsDeleted);

        builder.Ignore(bp => bp.CreatedBy);
        builder.Ignore(bp => bp.UpdatedBy);
    }
}
