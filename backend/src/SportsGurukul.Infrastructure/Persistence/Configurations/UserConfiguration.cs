using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.FullName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(u => u.ProfileImageUrl)
            .HasMaxLength(500);

        builder.Property(u => u.Status)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(u => u.AuthMethod)
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.PhoneNumber)
            .IsUnique()
            .HasDatabaseName("IX_Users_PhoneNumber");

        builder.HasIndex(u => u.Status)
            .HasDatabaseName("IX_Users_Status");

        builder.HasIndex(u => u.IsDeleted)
            .HasDatabaseName("IX_Users_IsDeleted");

        builder.Ignore(u => u.UserProfile);
        builder.Ignore(u => u.EmailVerificationTokens);
        builder.Ignore(u => u.PasswordResetTokens);

        var adminUserId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479");

        builder.HasData(
            new User
            {
                Id = adminUserId,
                Email = "admin@sportsgurukul.com",
                PhoneNumber = "+919000000001",
                PasswordHash = "ADMIN_SEED_HASH_TO_BE_REPLACED_ON_FIRST_LOGIN",
                FullName = "System Administrator",
                Status = UserStatus.Active,
                AuthMethod = AuthenticationMethod.EmailPassword,
                IsEmailVerified = true,
                FailedLoginAttempts = 0,
                IsDeleted = false
            });
    }
}
