using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingProgramSportConfiguration : IEntityTypeConfiguration<TrainingProgramSport>
{
    public void Configure(EntityTypeBuilder<TrainingProgramSport> builder)
    {
        builder.ToTable("TrainingProgramSports");

        builder.HasKey(ps => ps.Id);

        builder.HasIndex(ps => new { ps.TrainingProgramId, ps.SportId })
            .IsUnique()
            .HasDatabaseName("IX_TrainingProgramSports_ProgramId_SportId");

        builder.HasIndex(ps => ps.TrainingProgramId)
            .HasDatabaseName("IX_TrainingProgramSports_TrainingProgramId");

        builder.HasIndex(ps => ps.SportId)
            .HasDatabaseName("IX_TrainingProgramSports_SportId");

        builder.HasOne(ps => ps.TrainingProgram)
            .WithMany(p => p.ProgramSports)
            .HasForeignKey(ps => ps.TrainingProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Sport)
            .WithMany()
            .HasForeignKey(ps => ps.SportId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Ignore(ps => ps.CreatedBy);
        builder.Ignore(ps => ps.UpdatedBy);
    }
}
