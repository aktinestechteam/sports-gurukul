using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentTeamConfiguration : IEntityTypeConfiguration<TournamentTeam>
{
    public void Configure(EntityTypeBuilder<TournamentTeam> builder)
    {
        builder.ToTable("TournamentTeams");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.TeamName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(t => t.TeamCode)
            .HasMaxLength(30);

        builder.Property(t => t.RowVersion)
            .IsRowVersion();

        builder.HasIndex(t => t.TournamentId)
            .HasDatabaseName("IX_TournamentTeams_TournamentId");

        builder.HasIndex(t => t.AcademyId)
            .HasDatabaseName("IX_TournamentTeams_AcademyId");

        builder.HasIndex(t => new { t.TournamentId, t.TeamCode })
            .IsUnique()
            .HasDatabaseName("IX_TournamentTeams_TournamentId_TeamCode");

        builder.HasOne(t => t.Tournament)
            .WithMany(te => te.Teams)
            .HasForeignKey(t => t.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(t => t.Academy)
            .WithMany()
            .HasForeignKey(t => t.AcademyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(t => t.CreatedBy);
        builder.Ignore(t => t.UpdatedBy);
    }
}
