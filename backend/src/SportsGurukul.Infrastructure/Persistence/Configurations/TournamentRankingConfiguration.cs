using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentRankingConfiguration : IEntityTypeConfiguration<TournamentRanking>
{
    public void Configure(EntityTypeBuilder<TournamentRanking> builder)
    {
        builder.ToTable("TournamentRankings");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RowVersion)
            .IsRowVersion();

        builder.HasIndex(r => r.TournamentId)
            .HasDatabaseName("IX_TournamentRankings_TournamentId");

        builder.HasIndex(r => r.CategoryId)
            .HasDatabaseName("IX_TournamentRankings_CategoryId");

        builder.HasIndex(r => r.ParticipantId)
            .HasDatabaseName("IX_TournamentRankings_ParticipantId");

        builder.HasIndex(r => new { r.TournamentId, r.Rank })
            .HasDatabaseName("IX_TournamentRankings_TournamentId_Rank");

        builder.HasIndex(r => new { r.TournamentId, r.ParticipantId })
            .IsUnique()
            .HasDatabaseName("IX_TournamentRankings_TournamentId_ParticipantId");

        builder.HasOne(r => r.Tournament)
            .WithMany(t => t.Rankings)
            .HasForeignKey(r => r.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Category)
            .WithMany(c => c.Rankings)
            .HasForeignKey(r => r.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.Participant)
            .WithMany()
            .HasForeignKey(r => r.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
