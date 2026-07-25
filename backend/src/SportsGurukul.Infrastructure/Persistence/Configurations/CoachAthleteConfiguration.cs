using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachAthleteConfiguration : IEntityTypeConfiguration<CoachAthlete>
{
    public void Configure(EntityTypeBuilder<CoachAthlete> builder)
    {
        builder.ToTable("CoachAthlete");

        builder.HasKey(ca => ca.Id);

        builder.HasOne(ca => ca.Coach)
            .WithMany(c => c.CoachAthletes)
            .HasForeignKey(ca => ca.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ca => ca.Athlete)
            .WithMany()
            .HasForeignKey(ca => ca.AthleteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(ca => new { ca.CoachId, ca.AthleteId })
            .IsUnique()
            .HasFilter("\"IsActive\" = true");

        builder.HasIndex(ca => ca.AthleteId);
        builder.HasIndex(ca => ca.CoachId);

        builder.Property(ca => ca.AssignedDate)
            .IsRequired();
    }
}
