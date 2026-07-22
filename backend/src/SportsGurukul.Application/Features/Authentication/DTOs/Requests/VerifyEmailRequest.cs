using System.ComponentModel.DataAnnotations;

namespace SportsGurukul.Application.Features.Authentication.DTOs.Requests;

/// <summary>
/// Request to verify a user's email address using the token from the verification email.
/// </summary>
public class VerifyEmailRequest
{
    /// <summary>
    /// The verification token received in the email.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.example-verification-token</example>
    [Required]
    public string Token { get; set; } = string.Empty;
}
