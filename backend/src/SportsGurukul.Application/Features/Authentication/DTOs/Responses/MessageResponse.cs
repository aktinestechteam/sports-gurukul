namespace SportsGurukul.Application.Features.Authentication.DTOs.Responses;

/// <summary>
/// Generic message response returned by operations that don't return data.
/// </summary>
public class MessageResponse
{
    /// <summary>
    /// Human-readable message describing the result.
    /// </summary>
    /// <example>Email verified successfully.</example>
    public string Message { get; set; } = string.Empty;
}
