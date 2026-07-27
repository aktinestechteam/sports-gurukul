using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentAwardConfiguration : IEntityTypeConfiguration<TournamentAward>
{
    public void Configure(EntityTypeBuilder<TournamentAward> builder)
    {
        builder.ToTable("TournamentAwards");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AwardType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.AwardName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(a => a.ParticipantName)
            .HasMaxLength(200);

        builder.Property(a => a.TeamName)
            .HasMaxLength(200);

        builder.Property(a => a.Description)
            .HasMaxLength(2000);

        builder.Property(a => a.PrizeMoney)
            .HasPrecision(10, 2);

        builder.Property(a => a.CertificateUrl)
            .HasMaxLength(500);

        builder.Property(a => a.RowVersion)
            .IsRowVersion();

        builder.HasIndex(a => a.TournamentId)
            .HasDatabaseName("IX_TournamentAwards_TournamentId");

        builder.HasIndex(a => a.AwardType)
            .HasDatabaseName("IX_TournamentAwards_AwardType");

        builder.HasIndex(a => a.ParticipantId)
            .HasDatabaseName("IX_TournamentAwards_ParticipantId");

        builder.HasIndex(a => a.TeamId)
            .HasDatabaseName("IX_TournamentAwards_TeamId");

        builder.HasOne(a => a.Tournament)
            .WithMany(t => t.Awards)
            .HasForeignKey(a => a.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Participant)
            .WithMany()
            .HasForeignKey(a => a.ParticipantId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(a => a.Team)
            .WithMany()
            .HasForeignKey(a => a.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
