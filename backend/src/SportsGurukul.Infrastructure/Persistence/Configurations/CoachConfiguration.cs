using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class CoachConfiguration : IEntityTypeConfiguration<Coach>
{
    public void Configure(EntityTypeBuilder<Coach> builder)
    {
        builder.ToTable("Coaches");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CoachCode)
            .HasMaxLength(50);

        builder.Property(c => c.Biography)
            .HasMaxLength(2000);

        builder.Property(c => c.CurrentOrganization)
            .HasMaxLength(200);

        builder.Property(c => c.HighestQualification)
            .HasMaxLength(200);

        builder.Property(c => c.PreferredLanguage)
            .HasMaxLength(50);

        builder.Property(c => c.CoachingLevel)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.VerificationStatus)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(c => c.RowVersion)
            .IsRowVersion()
            .HasDefaultValueSql("E'\\\\x00'::bytea");

        builder.HasIndex(c => c.UserId)
            .IsUnique()
            .HasDatabaseName("IX_Coaches_UserId");

        builder.HasIndex(c => c.CoachCode)
            .IsUnique()
            .HasDatabaseName("IX_Coaches_CoachCode");

        builder.HasIndex(c => c.Status)
            .HasDatabaseName("IX_Coaches_Status");

        builder.HasIndex(c => c.CoachingLevel)
            .HasDatabaseName("IX_Coaches_CoachingLevel");

        builder.HasIndex(c => c.VerificationStatus)
            .HasDatabaseName("IX_Coaches_VerificationStatus");

        builder.HasIndex(c => new { c.Status, c.CoachingLevel })
            .HasDatabaseName("IX_Coaches_Status_CoachingLevel");

        builder.HasIndex(c => new { c.Status, c.CreatedAt })
            .HasDatabaseName("IX_Coaches_Status_CreatedAt");

        builder.HasIndex(c => c.YearsOfExperience)
            .HasDatabaseName("IX_Coaches_YearsOfExperience");

        builder.HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Coach>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Availability)
            .WithOne(a => a.Coach)
            .HasForeignKey<CoachAvailability>(a => a.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(c => c.Location)
            .WithOne(l => l.Coach)
            .HasForeignKey<CoachLocation>(l => l.CoachId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.Ignore(c => c.CreatedBy);
        builder.Ignore(c => c.UpdatedBy);

        builder.HasData(
            new Coach
            {
                Id = Guid.Parse("d1000000-0000-0000-0000-000000000001"),
                UserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
                CoachCode = "COACH-20250101-SEED01",
                RegistrationDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                Biography = "Seed coach profile for development.",
                YearsOfExperience = 5,
                CoachingLevel = CoachingLevel.Senior,
                Status = CoachStatus.Active,
                VerificationStatus = VerificationStatus.Verified,
                IsDeleted = false
            }
        );
    }
}
