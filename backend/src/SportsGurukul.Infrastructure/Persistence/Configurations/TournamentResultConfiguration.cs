using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentResultConfiguration : IEntityTypeConfiguration<TournamentResult>
{
    public void Configure(EntityTypeBuilder<TournamentResult> builder)
    {
        builder.ToTable("TournamentResults");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.WinnerName)
            .HasMaxLength(200);

        builder.Property(r => r.ResultDetails)
            .HasMaxLength(2000);

        builder.Property(r => r.Notes)
            .HasMaxLength(2000);

        builder.Property(r => r.RowVersion)
            .IsRowVersion();

        builder.HasIndex(r => r.TournamentId)
            .HasDatabaseName("IX_TournamentResults_TournamentId");

        builder.HasIndex(r => r.MatchId)
            .HasDatabaseName("IX_TournamentResults_MatchId");

        builder.HasIndex(r => r.WinnerId)
            .HasDatabaseName("IX_TournamentResults_WinnerId");

        builder.HasOne(r => r.Tournament)
            .WithMany()
            .HasForeignKey(r => r.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Match)
            .WithMany(m => m.Results)
            .HasForeignKey(r => r.MatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Winner)
            .WithMany()
            .HasForeignKey(r => r.WinnerId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
