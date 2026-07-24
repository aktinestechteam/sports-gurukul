using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachExperienceConfiguration : IEntityTypeConfiguration<CoachExperience>
{
    public void Configure(EntityTypeBuilder<CoachExperience> builder)
    {
        builder.ToTable("CoachExperiences");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Organization)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Role)
            .HasMaxLength(200);

        builder.Property(e => e.Sport)
            .HasMaxLength(100);

        builder.Property(e => e.Description)
            .HasMaxLength(2000);

        builder.HasIndex(e => e.CoachId)
            .HasDatabaseName("IX_CoachExperiences_CoachId");

        builder.HasOne(e => e.Coach)
            .WithMany(c => c.Experiences)
            .HasForeignKey(e => e.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);

        builder.HasData(
            new CoachExperience
            {
                Id = Guid.Parse("f1000000-0000-0000-0000-000000000001"),
                CoachId = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                Organization = "State Cricket Academy",
                Role = "Head Coach",
                Sport = "Cricket",
                StartDate = new DateTime(2020, 4, 1, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2024, 3, 31, 0, 0, 0, DateTimeKind.Utc),
                Description = "Led state-level cricket training program.",
                IsDeleted = false
            }
        );
    }
}
