using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("Achievements");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .HasMaxLength(200);

        builder.Property(a => a.Competition)
            .HasMaxLength(200);

        builder.Property(a => a.Position)
            .HasMaxLength(100);

        builder.Property(a => a.Level)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(a => a.CertificateUrl)
            .HasMaxLength(2000);

        builder.HasIndex(a => a.Title)
            .HasDatabaseName("IX_Achievements_Title");

        builder.HasIndex(a => a.Level)
            .HasDatabaseName("IX_Achievements_Level");

        builder.Ignore(a => a.CreatedBy);
        builder.Ignore(a => a.UpdatedBy);
    }
}
