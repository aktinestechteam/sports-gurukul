using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class TrainingMilestoneConfiguration : IEntityTypeConfiguration<TrainingMilestone>
{
    public void Configure(EntityTypeBuilder<TrainingMilestone> builder)
    {
        builder.ToTable("TrainingMilestones");

        builder.HasKey(m => m.Id);

        builder.Property(m => m.MilestoneName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(m => m.Description)
            .HasMaxLength(2000);

        builder.HasIndex(m => m.ProgramId)
            .HasDatabaseName("IX_TrainingMilestones_ProgramId");

        builder.HasIndex(m => new { m.ProgramId, m.WeekNumber })
            .HasDatabaseName("IX_TrainingMilestones_ProgramId_WeekNumber");

        builder.HasOne(m => m.Program)
            .WithMany(p => p.Milestones)
            .HasForeignKey(m => m.ProgramId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(m => m.CreatedBy);
        builder.Ignore(m => m.UpdatedBy);
    }
}
