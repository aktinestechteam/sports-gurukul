using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.Authentication.DTOs.Requests;
using SportsGurukul.Application.Features.Authentication.DTOs.Responses;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;
using Xunit;

namespace SportsGurukul.IntegrationTests;

public class AuthIntegrationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private const string ValidPassword = "Test@1234";
    private const string StrongPassword = "Str0ng!Pass#2024";
    private const string JwtSigningKey = "REPLACE-WITH-A-SECURE-SECRET-KEY-AT-LEAST-32-CHARS-LONG!!";

    #region Register

    [Fact]
    public async Task Register_ValidRequest_Returns201WithAuthResponse()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RegisterRequest
        {
            FullName = "Rahul Sharma",
            Email = "rahul@example.com",
            Password = ValidPassword,
            ConfirmPassword = ValidPassword,
            PhoneNumber = "+919876543210"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<AuthResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("rahul@example.com", content.Data!.Email);
        Assert.Equal("Rahul Sharma", content.Data.FullName);
        Assert.False(string.IsNullOrEmpty(content.Data.AccessToken));
        Assert.False(string.IsNullOrEmpty(content.Data.RefreshToken));
        Assert.Contains("Athlete", content.Data.Roles);
        Assert.Single(factory.GetUsers());
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "existing@example.com",
            FullName = "Existing User",
            PhoneNumber = "+919000000001",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        using var client = factory.CreateClient();

        var request = new RegisterRequest
        {
            FullName = "New User",
            Email = "existing@example.com",
            Password = ValidPassword,
            ConfirmPassword = ValidPassword
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WeakPassword_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RegisterRequest
        {
            FullName = "Weak Pass User",
            Email = "weak@example.com",
            Password = "weak",
            ConfirmPassword = "weak"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_PasswordMismatch_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RegisterRequest
        {
            FullName = "Mismatch User",
            Email = "mismatch@example.com",
            Password = ValidPassword,
            ConfirmPassword = "Different@123"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_EmptyFields_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RegisterRequest
        {
            FullName = "",
            Email = "",
            Password = "",
            ConfirmPassword = ""
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidEmail_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RegisterRequest
        {
            FullName = "Invalid Email User",
            Email = "not-an-email",
            Password = ValidPassword,
            ConfirmPassword = ValidPassword
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicatePhone_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "other@example.com",
            FullName = "Other User",
            PhoneNumber = "+919876543210",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        using var client = factory.CreateClient();

        var request = new RegisterRequest
        {
            FullName = "Phone User",
            Email = "phone@example.com",
            Password = ValidPassword,
            ConfirmPassword = ValidPassword,
            PhoneNumber = "+919876543210"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_WithoutPhone_Returns201()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RegisterRequest
        {
            FullName = "No Phone User",
            Email = "nophone@example.com",
            Password = ValidPassword,
            ConfirmPassword = ValidPassword
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    #endregion

    #region Login

    [Fact]
    public async Task Login_ValidCredentials_Returns200WithLoginResponse()
    {
        await using var factory = new TestApplicationFactory();
        var hasher = new MockPasswordHasher();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "login@example.com",
            FullName = "Login User",
            PhoneNumber = "+919000000002",
            PasswordHash = hasher.HashPassword(ValidPassword),
            Status = UserStatus.Active
        });
        using var client = factory.CreateClient();

        var request = new LoginRequest
        {
            Email = "login@example.com",
            Password = ValidPassword
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<LoginResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.Equal("login@example.com", content.Data!.Email);
        Assert.False(string.IsNullOrEmpty(content.Data.AccessToken));
        Assert.False(string.IsNullOrEmpty(content.Data.RefreshToken));
    }

    [Fact]
    public async Task Login_InvalidPassword_Returns401()
    {
        await using var factory = new TestApplicationFactory();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "login@example.com",
            FullName = "Login User",
            PhoneNumber = "+919000000003",
            PasswordHash = new MockPasswordHasher().HashPassword(ValidPassword),
            Status = UserStatus.Active
        });
        using var client = factory.CreateClient();

        var request = new LoginRequest
        {
            Email = "login@example.com",
            Password = "WrongPassword@1"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_NonExistentEmail_Returns401()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new LoginRequest
        {
            Email = "nonexistent@example.com",
            Password = ValidPassword
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_EmptyCredentials_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new LoginRequest
        {
            Email = "",
            Password = ""
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_InvalidEmailFormat_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new LoginRequest
        {
            Email = "not-an-email",
            Password = ValidPassword
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_LockedAccount_Returns401()
    {
        await using var factory = new TestApplicationFactory();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "locked@example.com",
            FullName = "Locked User",
            PhoneNumber = "+919000000004",
            PasswordHash = new MockPasswordHasher().HashPassword(ValidPassword),
            Status = UserStatus.Locked,
            LockoutEndAt = DateTime.UtcNow.AddMinutes(15)
        });
        using var client = factory.CreateClient();

        var request = new LoginRequest
        {
            Email = "locked@example.com",
            Password = ValidPassword
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_5FailedAttempts_LocksAccount()
    {
        await using var factory = new TestApplicationFactory();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "bruteforce@example.com",
            FullName = "Brute Force User",
            PhoneNumber = "+919000000005",
            PasswordHash = new MockPasswordHasher().HashPassword(ValidPassword),
            Status = UserStatus.Active,
            FailedLoginAttempts = 4
        });
        using var client = factory.CreateClient();

        var request = new LoginRequest
        {
            Email = "bruteforce@example.com",
            Password = "WrongPassword@1"
        };

        var response = await client.PostAsJsonAsync("/api/v1/auth/login", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var user = factory.GetUsers().First(u => u.Email == "bruteforce@example.com");
        Assert.Equal(UserStatus.Locked, user.Status);
    }

    #endregion

    #region Refresh Token

    [Fact]
    public async Task RefreshToken_ValidToken_Returns200WithNewTokens()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        var validRefreshToken = "valid-refresh-token-123";
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "refresh@example.com",
            FullName = "Refresh User",
            PhoneNumber = "+919000000006",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        factory.GetRefreshTokens().Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = validRefreshToken,
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });
        using var client = factory.CreateClient();

        var request = new RefreshTokenRequest { RefreshToken = validRefreshToken };
        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadFromJsonAsync<ApiResponse<TokenResponse>>(JsonOptions);
        Assert.NotNull(content);
        Assert.True(content!.Success);
        Assert.NotNull(content.Data);
        Assert.False(string.IsNullOrEmpty(content.Data!.AccessToken));
        Assert.False(string.IsNullOrEmpty(content.Data.RefreshToken));
    }

    [Fact]
    public async Task RefreshToken_InvalidToken_Returns401()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RefreshTokenRequest { RefreshToken = "nonexistent-token" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_EmptyToken_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new RefreshTokenRequest { RefreshToken = "" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_ExpiredToken_Returns401()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "expired@example.com",
            FullName = "Expired User",
            PhoneNumber = "+919000000007",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        factory.GetRefreshTokens().Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "expired-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        });
        using var client = factory.CreateClient();

        var request = new RefreshTokenRequest { RefreshToken = "expired-token" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_RevokedToken_Returns401()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "revoked@example.com",
            FullName = "Revoked User",
            PhoneNumber = "+919000000008",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        factory.GetRefreshTokens().Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "revoked-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(30),
            RevokedAt = DateTime.UtcNow.AddHours(-1)
        });
        using var client = factory.CreateClient();

        var request = new RefreshTokenRequest { RefreshToken = "revoked-token" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_InactiveUser_Returns401()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "inactive@example.com",
            FullName = "Inactive User",
            PhoneNumber = "+919000000009",
            PasswordHash = "hash",
            Status = UserStatus.Inactive
        });
        factory.GetRefreshTokens().Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "inactive-user-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });
        using var client = factory.CreateClient();

        var request = new RefreshTokenRequest { RefreshToken = "inactive-user-token" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/refresh-token", request);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region Logout

    [Fact]
    public async Task Logout_AuthenticatedUser_Returns204()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "logout@example.com",
            FullName = "Logout User",
            PhoneNumber = "+919000000010",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        using var client = factory.CreateClient();

        var token = GenerateTestJwtToken(userId);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/v1/auth/logout", new { });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task Logout_Unauthenticated_Returns401()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/v1/auth/logout", new { });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Logout_RevokesAllRefreshTokens()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "logout2@example.com",
            FullName = "Logout User 2",
            PhoneNumber = "+919000000011",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        factory.GetRefreshTokens().Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "token1",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });
        factory.GetRefreshTokens().Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = "token2",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddDays(30)
        });
        using var client = factory.CreateClient();

        var token = GenerateTestJwtToken(userId);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var response = await client.PostAsJsonAsync("/api/v1/auth/logout", new { });

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.All(factory.GetRefreshTokens().Where(t => t.UserId == userId),
            t => Assert.NotNull(t.RevokedAt));
    }

    #endregion

    #region Send Verification Email

    [Fact]
    public async Task SendVerificationEmail_ExistingUser_Returns200AndSendsEmail()
    {
        await using var factory = new TestApplicationFactory();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "verify@example.com",
            FullName = "Verify User",
            PhoneNumber = "+919000000012",
            PasswordHash = "hash",
            Status = UserStatus.Active,
            IsEmailVerified = false
        });
        using var client = factory.CreateClient();

        var request = new SendVerificationEmailRequest { Email = "verify@example.com" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/send-verification-email", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var emailService = factory.GetEmailService();
        Assert.Single(emailService.SentEmails);
    }

    [Fact]
    public async Task SendVerificationEmail_AlreadyVerified_Returns200NoEmail()
    {
        await using var factory = new TestApplicationFactory();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "already@example.com",
            FullName = "Already Verified",
            PhoneNumber = "+919000000013",
            PasswordHash = "hash",
            Status = UserStatus.Active,
            IsEmailVerified = true
        });
        using var client = factory.CreateClient();

        var request = new SendVerificationEmailRequest { Email = "already@example.com" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/send-verification-email", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var emailService = factory.GetEmailService();
        Assert.Empty(emailService.SentEmails);
    }

    [Fact]
    public async Task SendVerificationEmail_NonExistentEmail_Returns200NoEmail()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new SendVerificationEmailRequest { Email = "nonexistent@example.com" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/send-verification-email", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var emailService = factory.GetEmailService();
        Assert.Empty(emailService.SentEmails);
    }

    #endregion

    #region Verify Email

    [Fact]
    public async Task VerifyEmail_ValidToken_Returns200()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "verify@example.com",
            FullName = "Verify User",
            PhoneNumber = "+919000000014",
            PasswordHash = "hash",
            Status = UserStatus.Active,
            IsEmailVerified = false
        });
        factory.GetVerificationTokens().Add(new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-verify-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        });
        using var client = factory.CreateClient();

        var request = new VerifyEmailRequest { Token = "valid-verify-token" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/verify-email", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = factory.GetUsers().First(u => u.Id == userId);
        Assert.True(user.IsEmailVerified);
    }

    [Fact]
    public async Task VerifyEmail_InvalidToken_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new VerifyEmailRequest { Token = "invalid-token" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/verify-email", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task VerifyEmail_ExpiredToken_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "expired@example.com",
            FullName = "Expired Token User",
            PhoneNumber = "+919000000015",
            PasswordHash = "hash",
            Status = UserStatus.Active,
            IsEmailVerified = false
        });
        factory.GetVerificationTokens().Add(new EmailVerificationToken
        {
            Id = Guid.NewGuid(),
            Token = "expired-verify-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddHours(-1)
        });
        using var client = factory.CreateClient();

        var request = new VerifyEmailRequest { Token = "expired-verify-token" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/verify-email", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Forgot Password

    [Fact]
    public async Task ForgotPassword_ExistingUser_Returns200AndSendsEmail()
    {
        await using var factory = new TestApplicationFactory();
        factory.SeedUser(new User
        {
            Id = Guid.NewGuid(),
            Email = "forgot@example.com",
            FullName = "Forgot User",
            PhoneNumber = "+919000000016",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        using var client = factory.CreateClient();

        var request = new ForgotPasswordRequest { Email = "forgot@example.com" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/forgot-password", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var emailService = factory.GetEmailService();
        Assert.Single(emailService.SentEmails);
    }

    [Fact]
    public async Task ForgotPassword_NonExistentEmail_Returns200NoEmail()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new ForgotPasswordRequest { Email = "nonexistent@example.com" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/forgot-password", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var emailService = factory.GetEmailService();
        Assert.Empty(emailService.SentEmails);
    }

    [Fact]
    public async Task ForgotPassword_EmptyEmail_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new ForgotPasswordRequest { Email = "" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/forgot-password", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ForgotPassword_InvalidEmail_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new ForgotPasswordRequest { Email = "not-an-email" };
        var response = await client.PostAsJsonAsync("/api/v1/auth/forgot-password", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Reset Password

    [Fact]
    public async Task ResetPassword_ValidToken_Returns200()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "reset@example.com",
            FullName = "Reset User",
            PhoneNumber = "+919000000017",
            PasswordHash = new MockPasswordHasher().HashPassword(ValidPassword),
            Status = UserStatus.Active
        });
        factory.GetResetTokens().Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "valid-reset-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        });
        using var client = factory.CreateClient();

        var request = new ResetPasswordRequest
        {
            Token = "valid-reset-token",
            NewPassword = StrongPassword,
            ConfirmNewPassword = StrongPassword
        };
        var response = await client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_InvalidToken_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new ResetPasswordRequest
        {
            Token = "invalid-token",
            NewPassword = StrongPassword,
            ConfirmNewPassword = StrongPassword
        };
        var response = await client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_ExpiredToken_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "expired@example.com",
            FullName = "Expired Reset",
            PhoneNumber = "+919000000018",
            PasswordHash = "hash",
            Status = UserStatus.Active
        });
        factory.GetResetTokens().Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "expired-reset-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(-1)
        });
        using var client = factory.CreateClient();

        var request = new ResetPasswordRequest
        {
            Token = "expired-reset-token",
            NewPassword = StrongPassword,
            ConfirmNewPassword = StrongPassword
        };
        var response = await client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_PasswordMismatch_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new ResetPasswordRequest
        {
            Token = "some-token",
            NewPassword = StrongPassword,
            ConfirmNewPassword = "Different@123"
        };
        var response = await client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_WeakPassword_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new ResetPasswordRequest
        {
            Token = "some-token",
            NewPassword = "weak",
            ConfirmNewPassword = "weak"
        };
        var response = await client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ResetPassword_UnlocksLockedAccount()
    {
        await using var factory = new TestApplicationFactory();
        var userId = Guid.NewGuid();
        factory.SeedUser(new User
        {
            Id = userId,
            Email = "locked@example.com",
            FullName = "Locked Reset",
            PhoneNumber = "+919000000019",
            PasswordHash = "hash",
            Status = UserStatus.Locked,
            LockoutEndAt = DateTime.UtcNow.AddMinutes(15)
        });
        factory.GetResetTokens().Add(new PasswordResetToken
        {
            Id = Guid.NewGuid(),
            Token = "unlock-reset-token",
            UserId = userId,
            ExpiresAt = DateTime.UtcNow.AddMinutes(30)
        });
        using var client = factory.CreateClient();

        var request = new ResetPasswordRequest
        {
            Token = "unlock-reset-token",
            NewPassword = StrongPassword,
            ConfirmNewPassword = StrongPassword
        };
        var response = await client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var user = factory.GetUsers().First(u => u.Id == userId);
        Assert.Equal(UserStatus.Active, user.Status);
    }

    [Fact]
    public async Task ResetPassword_EmptyFields_Returns400()
    {
        await using var factory = new TestApplicationFactory();
        using var client = factory.CreateClient();

        var request = new ResetPasswordRequest
        {
            Token = "",
            NewPassword = "",
            ConfirmNewPassword = ""
        };
        var response = await client.PostAsJsonAsync("/api/v1/auth/reset-password", request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    private static string GenerateTestJwtToken(Guid userId)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var key = System.Text.Encoding.UTF8.GetBytes(JwtSigningKey);
        var descriptor = new Microsoft.IdentityModel.Tokens.SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new[]
            {
                new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.NameIdentifier, userId.ToString())
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = "SportsGurukul",
            Audience = "SportsGurukul",
            SigningCredentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
                new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
                Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256)
        };
        var token = handler.CreateToken(descriptor);
        return handler.WriteToken(token);
    }
}