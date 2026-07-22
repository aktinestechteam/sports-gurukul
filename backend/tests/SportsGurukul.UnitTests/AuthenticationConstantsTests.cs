using SportsGurukul.Application.Features.Authentication.Constants;

namespace SportsGurukul.UnitTests;

public class AuthenticationConstantsTests
{
    [Fact]
    public void EmailVerificationTokenExpiryHours_ShouldBe24()
    {
        Assert.Equal(24, AuthenticationConstants.EmailVerificationTokenExpiryHours);
    }

    [Fact]
    public void PasswordResetTokenExpiryMinutes_ShouldBe30()
    {
        Assert.Equal(30, AuthenticationConstants.PasswordResetTokenExpiryMinutes);
    }

    [Fact]
    public void TokenByteLength_ShouldBe32()
    {
        Assert.Equal(32, AuthenticationConstants.TokenByteLength);
    }

    [Fact]
    public void MaxFailedLoginAttempts_ShouldBe5()
    {
        Assert.Equal(5, AuthenticationConstants.MaxFailedLoginAttempts);
    }

    [Fact]
    public void PasswordMinLength_ShouldBe8()
    {
        Assert.Equal(8, AuthenticationConstants.PasswordMinLength);
    }
}
