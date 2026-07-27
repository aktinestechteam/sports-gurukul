using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentCourtConfiguration : IEntityTypeConfiguration<TournamentCourt>
{
    public void Configure(EntityTypeBuilder<TournamentCourt> builder)
    {
        builder.ToTable("TournamentCourts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CourtName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.CourtType)
            .HasMaxLength(50);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.RowVersion)
            .IsRowVersion();

        builder.HasIndex(c => c.TournamentVenueId)
            .HasDatabaseName("IX_TournamentCourts_TournamentVenueId");

        builder.HasOne(c => c.TournamentVenue)
            .WithMany(v => v.Courts)
            .HasForeignKey(c => c.TournamentVenueId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);
    }
}
