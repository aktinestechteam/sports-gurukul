using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachEducationConfiguration : IEntityTypeConfiguration<CoachEducation>
{
    public void Configure(EntityTypeBuilder<CoachEducation> builder)
    {
        builder.ToTable("CoachEducation");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Degree)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Institution)
            .HasMaxLength(200);

        builder.Property(e => e.FieldOfStudy)
            .HasMaxLength(200);

        builder.HasIndex(e => e.CoachId)
            .HasDatabaseName("IX_CoachEducation_CoachId");

        builder.HasOne(e => e.Coach)
            .WithMany(c => c.Education)
            .HasForeignKey(e => e.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(e => e.CreatedBy);
        builder.Ignore(e => e.UpdatedBy);

        builder.HasData(
            new CoachEducation
            {
                Id = Guid.Parse("a2000000-0000-0000-0000-000000000001"),
                CoachId = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                Degree = "Bachelor of Physical Education",
                Institution = "National Institute of Sports",
                FieldOfStudy = "Sports Coaching",
                YearCompleted = 2018,
                IsDeleted = false
            }
        );
    }
}
