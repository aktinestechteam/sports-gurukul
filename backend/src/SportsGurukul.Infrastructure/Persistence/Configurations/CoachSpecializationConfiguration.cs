using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachSpecializationConfiguration : IEntityTypeConfiguration<CoachSpecialization>
{
    public void Configure(EntityTypeBuilder<CoachSpecialization> builder)
    {
        builder.ToTable("CoachSpecializations");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.SpecializationName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(s => s.Description)
            .HasMaxLength(500);

        builder.HasIndex(s => s.CoachId)
            .HasDatabaseName("IX_CoachSpecializations_CoachId");

        builder.HasIndex(s => new { s.CoachId, s.SpecializationName })
            .IsUnique()
            .HasDatabaseName("IX_CoachSpecializations_CoachId_Name");

        builder.HasOne(s => s.Coach)
            .WithMany(c => c.Specializations)
            .HasForeignKey(s => s.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(s => s.CreatedBy);
        builder.Ignore(s => s.UpdatedBy);

        builder.HasData(
            new CoachSpecialization
            {
                Id = Guid.Parse("d2000000-0000-0000-0000-000000000001"),
                CoachId = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                SpecializationName = "Fast Bowling",
                Description = "Specialized in pace and swing bowling techniques.",
                IsDeleted = false
            },
            new CoachSpecialization
            {
                Id = Guid.Parse("d2000000-0000-0000-0000-000000000002"),
                CoachId = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                SpecializationName = "Fielding",
                Description = "Specialized in fielding drills and athleticism.",
                IsDeleted = false
            }
        );
    }
}
