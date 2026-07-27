using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingGoalConfiguration : IEntityTypeConfiguration<TrainingGoal>
{
    public void Configure(EntityTypeBuilder<TrainingGoal> builder)
    {
        builder.ToTable("TrainingGoals");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.GoalName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(g => g.Description)
            .HasMaxLength(2000);

        builder.HasIndex(g => g.ProgramId)
            .HasDatabaseName("IX_TrainingGoals_ProgramId");

        builder.HasIndex(g => new { g.ProgramId, g.TargetWeek })
            .HasDatabaseName("IX_TrainingGoals_ProgramId_TargetWeek");

        builder.HasOne(g => g.Program)
            .WithMany(p => p.Goals)
            .HasForeignKey(g => g.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(g => g.CreatedBy);
        builder.Ignore(g => g.UpdatedBy);
    }
}
