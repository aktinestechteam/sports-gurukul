using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class UserPreferenceConfiguration : IEntityTypeConfiguration<UserPreference>
{
    public void Configure(EntityTypeBuilder<UserPreference> builder)
    {
        builder.ToTable("UserPreferences");

        builder.HasKey(up => up.Id);

        builder.Property(up => up.Language)
            .IsRequired()
            .HasMaxLength(10);

        builder.Property(up => up.Theme)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(up => up.TimeZone)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(up => up.UserProfileId)
            .IsUnique()
            .HasDatabaseName("IX_UserPreferences_UserProfileId");

        builder.HasOne(up => up.UserProfile)
            .WithOne(p => p.UserPreference)
            .HasForeignKey<UserPreference>(up => up.UserProfileId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(up => up.CreatedBy);
        builder.Ignore(up => up.UpdatedBy);

        var adminUserProfileId = Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7");
        var adminPreferenceId = Guid.Parse("d4a5c5b5-3b2f-4e8a-9f1c-8b7d6e5f4a3b");

        builder.HasData(
            new UserPreference
            {
                Id = adminPreferenceId,
                UserProfileId = adminUserProfileId,
                Language = "en",
                Theme = Theme.Light,
                TimeZone = "Asia/Kolkata",
                EmailNotifications = true,
                PushNotifications = true,
                SmsNotifications = false,
                MarketingEmails = false,
                ProfileVisibility = true,
                ShowOnlineStatus = true,
                IsDeleted = false
            });
    }
}
