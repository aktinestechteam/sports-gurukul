using System.ComponentModel.DataAnnotations;

namespace SportsGurukul.Application.Features.Authentication.DTOs.Requests;

/// <summary>
/// Request to reset a user's password using the token from the reset email.
/// After reset, all active refresh tokens are revoked (forces re-login on all devices).
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>
    /// The reset token received in the email.
    /// </summary>
    /// <example>eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.example-reset-token</example>
    [Required]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// The new password. Must be at least 8 characters with uppercase, lowercase, number, and special character.
    /// </summary>
    /// <example>NewSecureP@ss1</example>
    [Required]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// Confirmation of the new password. Must match NewPassword.
    /// </summary>
    /// <example>NewSecureP@ss1</example>
    [Required]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
