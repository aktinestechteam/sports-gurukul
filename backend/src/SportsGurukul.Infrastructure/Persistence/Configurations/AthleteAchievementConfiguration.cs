using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AthleteAchievementConfiguration : IEntityTypeConfiguration<AthleteAchievement>
{
    public void Configure(EntityTypeBuilder<AthleteAchievement> builder)
    {
        builder.ToTable("AthleteAchievements");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Notes)
            .HasMaxLength(500);

        builder.HasIndex(a => new { a.AthleteId, a.AchievementId })
            .IsUnique()
            .HasDatabaseName("IX_AthleteAchievements_AthleteId_AchievementId");

        builder.HasIndex(a => a.AthleteId)
            .HasDatabaseName("IX_AthleteAchievements_AthleteId");

        builder.HasIndex(a => a.AchievementId)
            .HasDatabaseName("IX_AthleteAchievements_AchievementId");

        builder.HasOne(a => a.Athlete)
            .WithMany(a => a.AthleteAchievements)
            .HasForeignKey(a => a.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(a => a.Achievement)
            .WithMany(a => a.AthleteAchievements)
            .HasForeignKey(a => a.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
