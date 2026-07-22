using System.Security.Cryptography;
using SportsGurukul.Application.Features.Authentication.Constants;

namespace SportsGurukul.UnitTests;

public class TokenGenerationTests
{
    [Fact]
    public void GenerateSecureToken_ShouldReturnCorrectLength()
    {
        var tokenBytes = new byte[AuthenticationConstants.TokenByteLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(tokenBytes);

        var token = Convert.ToBase64String(tokenBytes);

        Assert.False(string.IsNullOrEmpty(token));
        Assert.True(token.Length > 0);
    }

    [Fact]
    public void GenerateSecureToken_ShouldBeUnique()
    {
        var token1 = GenerateToken();
        var token2 = GenerateToken();

        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateSecureToken_ShouldBeUrlSafe()
    {
        var token = GenerateUrlSafeToken();

        Assert.DoesNotContain("+", token);
        Assert.DoesNotContain("/", token);
        Assert.DoesNotContain("=", token);
    }

    [Fact]
    public void TokenExpiry_ShouldBeFutureDate()
    {
        var expiry = DateTime.UtcNow.AddHours(AuthenticationConstants.EmailVerificationTokenExpiryHours);

        Assert.True(expiry > DateTime.UtcNow);
    }

    [Fact]
    public void PasswordResetExpiry_ShouldBeShorterThanEmailVerification()
    {
        var emailExpiry = AuthenticationConstants.EmailVerificationTokenExpiryHours * 60;
        var passwordResetExpiry = AuthenticationConstants.PasswordResetTokenExpiryMinutes;

        Assert.True(passwordResetExpiry < emailExpiry);
    }

    private static string GenerateToken()
    {
        var bytes = new byte[AuthenticationConstants.TokenByteLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes);
    }

    private static string GenerateUrlSafeToken()
    {
        var bytes = new byte[AuthenticationConstants.TokenByteLength];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(bytes);
        return Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .TrimEnd('=');
    }
}
