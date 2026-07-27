using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentParticipantConfiguration : IEntityTypeConfiguration<TournamentParticipant>
{
    public void Configure(EntityTypeBuilder<TournamentParticipant> builder)
    {
        builder.ToTable("TournamentParticipants");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.ParticipantType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(p => p.ParticipantName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.SeedNumber)
            .HasMaxLength(10);

        builder.Property(p => p.RowVersion)
            .IsRowVersion();

        builder.HasIndex(p => p.TournamentId)
            .HasDatabaseName("IX_TournamentParticipants_TournamentId");

        builder.HasIndex(p => p.CategoryId)
            .HasDatabaseName("IX_TournamentParticipants_CategoryId");

        builder.HasIndex(p => p.ParticipantType)
            .HasDatabaseName("IX_TournamentParticipants_ParticipantType");

        builder.HasIndex(p => p.AthleteId)
            .HasDatabaseName("IX_TournamentParticipants_AthleteId");

        builder.HasIndex(p => p.TeamId)
            .HasDatabaseName("IX_TournamentParticipants_TeamId");

        builder.HasIndex(p => p.AcademyId)
            .HasDatabaseName("IX_TournamentParticipants_AcademyId");

        builder.HasOne(p => p.Tournament)
            .WithMany(t => t.Participants)
            .HasForeignKey(p => p.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Participants)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Athlete)
            .WithMany()
            .HasForeignKey(p => p.AthleteId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Team)
            .WithMany(t => t.Participants)
            .HasForeignKey(p => p.TeamId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(p => p.Academy)
            .WithMany()
            .HasForeignKey(p => p.AcademyId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.Ignore(p => p.CreatedBy);
        builder.Ignore(p => p.UpdatedBy);
    }
}
