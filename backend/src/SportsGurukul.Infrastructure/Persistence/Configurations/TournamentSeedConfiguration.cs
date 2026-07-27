using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentSeedConfiguration : IEntityTypeConfiguration<TournamentSeed>
{
    public void Configure(EntityTypeBuilder<TournamentSeed> builder)
    {
        builder.ToTable("TournamentSeeds");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SeedSource)
            .HasMaxLength(100);

        builder.Property(s => s.RowVersion)
            .IsRowVersion();

        builder.HasIndex(s => s.TournamentId)
            .HasDatabaseName("IX_TournamentSeeds_TournamentId");

        builder.HasIndex(s => s.CategoryId)
            .HasDatabaseName("IX_TournamentSeeds_CategoryId");

        builder.HasIndex(s => s.ParticipantId)
            .HasDatabaseName("IX_TournamentSeeds_ParticipantId");

        builder.HasIndex(s => new { s.TournamentId, s.SeedPosition })
            .IsUnique()
            .HasDatabaseName("IX_TournamentSeeds_TournamentId_SeedPosition");

        builder.HasOne(s => s.Tournament)
            .WithMany()
            .HasForeignKey(s => s.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Category)
            .WithMany()
            .HasForeignKey(s => s.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(s => s.Participant)
            .WithMany(p => p.Seeds)
            .HasForeignKey(s => s.ParticipantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
