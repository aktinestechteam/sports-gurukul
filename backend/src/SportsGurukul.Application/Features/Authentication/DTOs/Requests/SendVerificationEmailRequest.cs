using System.ComponentModel.DataAnnotations;

namespace SportsGurukul.Application.Features.Authentication.DTOs.Requests;

/// <summary>
/// Request to send a verification email to the specified address.
/// </summary>
public class SendVerificationEmailRequest
{
    /// <summary>
    /// The email address to send the verification link to.
    /// </summary>
    /// <example>john.doe@example.com</example>
    [Required]
    public string Email { get; set; } = string.Empty;
}
