using System.ComponentModel.DataAnnotations;

namespace SportsGurukul.Application.Features.Authentication.DTOs.Requests;

/// <summary>
/// Request to send a password reset email to the specified address.
/// Always returns success regardless of whether the email exists (prevents email enumeration).
/// </summary>
public class ForgotPasswordRequest
{
    /// <summary>
    /// The email address associated with the account.
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required]
    public string Email { get; set; } = string.Empty;
}
