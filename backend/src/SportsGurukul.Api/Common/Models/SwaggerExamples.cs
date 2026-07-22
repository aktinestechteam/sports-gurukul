using SportsGurukul.Application.Features.Authentication.DTOs.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

public class RegisterRequestExample : IExamplesProvider<RegisterRequest>
{
    public RegisterRequest GetExamples() => new()
    {
        FullName = "John Doe",
        Email = "john.doe@example.com",
        Password = "SecureP@ss1",
        ConfirmPassword = "SecureP@ss1",
        PhoneNumber = "+919876543210"
    };
}

public class LoginRequestExample : IExamplesProvider<LoginRequest>
{
    public LoginRequest GetExamples() => new()
    {
        Email = "john.doe@example.com",
        Password = "SecureP@ss1"
    };
}

public class RefreshTokenRequestExample : IExamplesProvider<RefreshTokenRequest>
{
    public RefreshTokenRequest GetExamples() => new()
    {
        RefreshToken = "dGhpcyBpcyBhIHJlZnJlc2ggdG9rZW4gZXhhbXBsZQ=="
    };
}

public class SendVerificationEmailRequestExample : IExamplesProvider<SendVerificationEmailRequest>
{
    public SendVerificationEmailRequest GetExamples() => new()
    {
        Email = "john.doe@example.com"
    };
}

public class VerifyEmailRequestExample : IExamplesProvider<VerifyEmailRequest>
{
    public VerifyEmailRequest GetExamples() => new()
    {
        Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.example-verification-token"
    };
}

public class ForgotPasswordRequestExample : IExamplesProvider<ForgotPasswordRequest>
{
    public ForgotPasswordRequest GetExamples() => new()
    {
        Email = "john.doe@example.com"
    };
}

public class ResetPasswordRequestExample : IExamplesProvider<ResetPasswordRequest>
{
    public ResetPasswordRequest GetExamples() => new()
    {
        Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.example-reset-token",
        NewPassword = "NewSecureP@ss1",
        ConfirmNewPassword = "NewSecureP@ss1"
    };
}
