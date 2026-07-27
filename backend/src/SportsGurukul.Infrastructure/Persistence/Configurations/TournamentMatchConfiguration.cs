using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentMatchConfiguration : IEntityTypeConfiguration<TournamentMatch>
{
    public void Configure(EntityTypeBuilder<TournamentMatch> builder)
    {
        builder.ToTable("TournamentMatches");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.HomeParticipantName)
            .HasMaxLength(200);

        builder.Property(m => m.AwayParticipantName)
            .HasMaxLength(200);

        builder.Property(m => m.ScheduledTime)
            .HasPrecision(0);

        builder.Property(m => m.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(m => m.ScoreDetails)
            .HasMaxLength(2000);

        builder.Property(m => m.WinnerName)
            .HasMaxLength(200);

        builder.Property(m => m.Notes)
            .HasMaxLength(2000);

        builder.Property(m => m.RowVersion)
            .IsRowVersion();

        builder.HasIndex(m => m.TournamentId)
            .HasDatabaseName("IX_TournamentMatches_TournamentId");

        builder.HasIndex(m => m.TournamentStageId)
            .HasDatabaseName("IX_TournamentMatches_TournamentStageId");

        builder.HasIndex(m => m.TournamentRoundId)
            .HasDatabaseName("IX_TournamentMatches_TournamentRoundId");

        builder.HasIndex(m => m.TournamentVenueId)
            .HasDatabaseName("IX_TournamentMatches_TournamentVenueId");

        builder.HasIndex(m => m.TournamentCourtId)
            .HasDatabaseName("IX_TournamentMatches_TournamentCourtId");

        builder.HasIndex(m => m.Status)
            .HasDatabaseName("IX_TournamentMatches_Status");

        builder.HasIndex(m => m.ScheduledDate)
            .HasDatabaseName("IX_TournamentMatches_ScheduledDate");

        builder.HasIndex(m => new { m.TournamentId, m.MatchNumber })
            .IsUnique()
            .HasDatabaseName("IX_TournamentMatches_TournamentId_MatchNumber");

        builder.HasOne(m => m.Tournament)
            .WithMany()
            .HasForeignKey(m => m.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(m => m.TournamentStage)
            .WithMany()
            .HasForeignKey(m => m.TournamentStageId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.TournamentRound)
            .WithMany(r => r.Matches)
            .HasForeignKey(m => m.TournamentRoundId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.TournamentVenue)
            .WithMany()
            .HasForeignKey(m => m.TournamentVenueId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.TournamentCourt)
            .WithMany()
            .HasForeignKey(m => m.TournamentCourtId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.HomeParticipant)
            .WithMany(p => p.HomeMatches)
            .HasForeignKey(m => m.HomeParticipantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.AwayParticipant)
            .WithMany(p => p.AwayMatches)
            .HasForeignKey(m => m.AwayParticipantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(m => m.Winner)
            .WithMany(p => p.WonMatches)
            .HasForeignKey(m => m.WinnerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);
    }
}
