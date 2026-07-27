using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TournamentStageConfiguration : IEntityTypeConfiguration<TournamentStage>
{
    public void Configure(EntityTypeBuilder<TournamentStage> builder)
    {
        builder.ToTable("TournamentStages");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.StageName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(s => s.StageType)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.RowVersion)
            .IsRowVersion();

        builder.HasIndex(s => s.TournamentId)
            .HasDatabaseName("IX_TournamentStages_TournamentId");

        builder.HasIndex(s => s.StageOrder)
            .HasDatabaseName("IX_TournamentStages_StageOrder");

        builder.HasOne(s => s.Tournament)
            .WithMany(t => t.Stages)
            .HasForeignKey(s => s.TournamentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);
    }
}
