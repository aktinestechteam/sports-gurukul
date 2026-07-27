using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentSportConfiguration : IEntityTypeConfiguration<TournamentSport>
{
    public void Configure(EntityTypeBuilder<TournamentSport> builder)
    {
        builder.ToTable("TournamentSports");

        builder.HasKey(ts => ts.Id);

        builder.Property(ts => ts.SportName)
            .HasMaxLength(100);

        builder.Property(ts => ts.RowVersion)
            .IsRowVersion();

        builder.HasIndex(ts => ts.TournamentId)
            .HasDatabaseName("IX_TournamentSports_TournamentId");

        builder.HasIndex(ts => ts.SportId)
            .HasDatabaseName("IX_TournamentSports_SportId");

        builder.HasOne(ts => ts.Tournament)
            .WithMany(t => t.TournamentSports)
            .HasForeignKey(ts => ts.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ts => ts.Sport)
            .WithMany()
            .HasForeignKey(ts => ts.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(ts => ts.CreatedBy);
        builder.Ignore(ts => ts.UpdatedBy);
    }
}
