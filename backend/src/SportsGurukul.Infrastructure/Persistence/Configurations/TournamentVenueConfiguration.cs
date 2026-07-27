using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentVenueConfiguration : IEntityTypeConfiguration<TournamentVenue>
{
    public void Configure(EntityTypeBuilder<TournamentVenue> builder)
    {
        builder.ToTable("TournamentVenues");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.VenueName)
            .HasMaxLength(200);

        builder.Property(v => v.Address)
            .HasMaxLength(500);

        builder.Property(v => v.RowVersion)
            .IsRowVersion();

        builder.HasIndex(v => v.TournamentId)
            .HasDatabaseName("IX_TournamentVenues_TournamentId");

        builder.HasIndex(v => v.FacilityId)
            .HasDatabaseName("IX_TournamentVenues_FacilityId");

        builder.HasOne(v => v.Tournament)
            .WithMany(t => t.Venues)
            .HasForeignKey(v => v.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(v => v.Facility)
            .WithMany()
            .HasForeignKey(v => v.FacilityId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(v => v.CreatedBy);
        builder.Ignore(v => v.UpdatedBy);
    }
}
