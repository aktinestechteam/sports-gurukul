using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentMatchSetConfiguration : IEntityTypeConfiguration<TournamentMatchSet>
{
    public void Configure(EntityTypeBuilder<TournamentMatchSet> builder)
    {
        builder.ToTable("TournamentMatchSets");

        builder.HasKey(ms => ms.Id);

        builder.Property(ms => ms.SetDetails)
            .HasMaxLength(1000);

        builder.Property(ms => ms.WinnerName)
            .HasMaxLength(200);

        builder.Property(ms => ms.RowVersion)
            .IsRowVersion();

        builder.HasIndex(ms => ms.TournamentMatchId)
            .HasDatabaseName("IX_TournamentMatchSets_TournamentMatchId");

        builder.HasIndex(ms => new { ms.TournamentMatchId, ms.SetNumber })
            .IsUnique()
            .HasDatabaseName("IX_TournamentMatchSets_TournamentMatchId_SetNumber");

        builder.HasOne(ms => ms.TournamentMatch)
            .WithMany(m => m.Sets)
            .HasForeignKey(ms => ms.TournamentMatchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(ms => ms.CreatedBy);
        builder.Ignore(ms => ms.UpdatedBy);
    }
}
