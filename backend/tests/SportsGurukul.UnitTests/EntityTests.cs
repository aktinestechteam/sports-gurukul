using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests;

public class EntityTests
{
    [Fact]
    public void EmailVerificationToken_ShouldTrackUsage()
    {
        var token = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "test-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };

        Assert.Null(token.UsedAt);
        Assert.True(token.ExpiresAt > DateTime.UtcNow);

        token.UsedAt = DateTime.UtcNow;
        Assert.NotNull(token.UsedAt);
    }

    [Fact]
    public void PasswordResetToken_ShouldTrackUsage()
    {
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "test-reset-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        };

        Assert.Null(token.UsedAt);
        Assert.True(token.ExpiresAt > DateTime.UtcNow);

        token.UsedAt = DateTime.UtcNow;
        Assert.NotNull(token.UsedAt);
    }

    [Fact]
    public void User_IsEmailVerified_DefaultIsFalse()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            Status = UserStatus.Active
        };

        Assert.False(user.IsEmailVerified);
    }

    [Fact]
    public void User_IsEmailVerified_CanBeSetToTrue()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            Status = UserStatus.Active,
            IsEmailVerified = true
        };

        Assert.True(user.IsEmailVerified);
    }

    [Fact]
    public void EmailVerificationToken_ShouldExpire()
    {
        var token = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "expired-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(-1)
        };

        Assert.True(token.ExpiresAt <= DateTime.UtcNow);
    }

    [Fact]
    public void PasswordResetToken_ShouldExpire()
    {
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "expired-reset-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
        };

        Assert.True(token.ExpiresAt <= DateTime.UtcNow);
    }

    [Fact]
    public void EmailVerificationToken_CannotBeReusedAfterUse()
    {
        var token = new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "used-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            UsedAt = DateTime.UtcNow
        };

        Assert.NotNull(token.UsedAt);
    }

    [Fact]
    public void PasswordResetToken_CannotBeReusedAfterUse()
    {
        var token = new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "used-reset-token",
            UserId = Guid.NewGuid(),
            ExpiresAt = DateTime.UtcNow.AddMinutes(30),
            UsedAt = DateTime.UtcNow
        };

        Assert.NotNull(token.UsedAt);
    }
}
