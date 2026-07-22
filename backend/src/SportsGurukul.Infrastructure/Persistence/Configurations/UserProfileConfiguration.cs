using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.ToTable("UserProfiles");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.Bio)
            .HasMaxLength(2000);

        builder.Property(up => up.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(up => up.CoverImageUrl)
            .HasMaxLength(500);

        builder.Property(up => up.Height)
            .HasMaxLength(20);

        builder.Property(up => up.Weight)
            .HasMaxLength(20);

        builder.Property(up => up.PreferredSport)
            .HasMaxLength(100);

        builder.Property(up => up.ExperienceLevel)
            .HasMaxLength(50);

        builder.Property(up => up.Gender)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(up => up.UserId)
            .IsUnique()
            .HasDatabaseName("IX_UserProfiles_UserId");

        builder.HasIndex(up => up.IsDeleted)
            .HasDatabaseName("IX_UserProfiles_IsDeleted");

        builder.HasOne(up => up.User)
            .WithOne(u => u.UserProfile)
            .HasForeignKey<UserProfile>(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(up => up.CreatedBy);
        builder.Ignore(up => up.UpdatedBy);

        var adminUserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");
        var adminUserProfileId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");

        builder.HasData(
            new UserProfile
            {
                Id = adminUserProfileId,
                UserId = adminUserId,
                Gender = Gender.PreferNotToSay,
                Bio = "System administrator account",
                PreferredSport = "All",
                ExperienceLevel = "Expert",
                IsDeleted = false
            });
    }
}
