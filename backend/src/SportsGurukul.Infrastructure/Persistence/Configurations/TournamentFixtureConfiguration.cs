using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentFixtureConfiguration : IEntityTypeConfiguration<TournamentFixture>
{
    public void Configure(EntityTypeBuilder<TournamentFixture> builder)
    {
        builder.ToTable("TournamentFixtures");

        builder.HasKey(f => f.Id);

        builder.Property(f => f.ScheduledTime)
            .HasPrecision(0);

        builder.Property(f => f.HomeTeamName)
            .HasMaxLength(200);

        builder.Property(f => f.AwayTeamName)
            .HasMaxLength(200);

        builder.Property(f => f.Notes)
            .HasMaxLength(2000);

        builder.Property(f => f.RowVersion)
            .IsRowVersion();

        builder.HasIndex(f => f.TournamentId)
            .HasDatabaseName("IX_TournamentFixtures_TournamentId");

        builder.HasIndex(f => f.TournamentStageId)
            .HasDatabaseName("IX_TournamentFixtures_TournamentStageId");

        builder.HasIndex(f => f.VenueId)
            .HasDatabaseName("IX_TournamentFixtures_VenueId");

        builder.HasIndex(f => f.CourtId)
            .HasDatabaseName("IX_TournamentFixtures_CourtId");

        builder.HasIndex(f => new { f.TournamentId, f.FixtureNumber })
            .IsUnique()
            .HasDatabaseName("IX_TournamentFixtures_TournamentId_FixtureNumber");

        builder.HasOne(f => f.Tournament)
            .WithMany()
            .HasForeignKey(f => f.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(f => f.TournamentStage)
            .WithMany()
            .HasForeignKey(f => f.TournamentStageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(f => f.Venue)
            .WithMany()
            .HasForeignKey(f => f.VenueId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(f => f.Court)
            .WithMany()
            .HasForeignKey(f => f.CourtId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(f => f.CreatedBy);
        builder.Ignore(f => f.UpdatedBy);
    }
}
