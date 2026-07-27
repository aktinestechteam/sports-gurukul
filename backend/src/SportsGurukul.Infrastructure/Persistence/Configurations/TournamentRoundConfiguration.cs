using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentRoundConfiguration : IEntityTypeConfiguration<TournamentRound>
{
    public void Configure(EntityTypeBuilder<TournamentRound> builder)
    {
        builder.ToTable("TournamentRounds");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.RoundName)
            .HasMaxLength(100);

        builder.Property(r => r.RowVersion)
            .IsRowVersion();

        builder.HasIndex(r => r.TournamentStageId)
            .HasDatabaseName("IX_TournamentRounds_TournamentStageId");

        builder.HasIndex(r => r.RoundNumber)
            .HasDatabaseName("IX_TournamentRounds_RoundNumber");

        builder.HasOne(r => r.TournamentStage)
            .WithMany(s => s.Rounds)
            .HasForeignKey(r => r.TournamentStageId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(r => r.CreatedBy);
        builder.Ignore(r => r.UpdatedBy);
    }
}
