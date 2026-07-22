using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

/// <summary>
/// Request body for updating the current user's preferences.
/// All fields are optional — only supplied fields are applied.
/// </summary>
public class UpdateUserPreferenceRequest
{
    /// <summary>Preferred language code (ISO 639-1, e.g. "en", "hi").</summary>
    /// <example>en</example>
    public string? Language { get; set; }

    /// <summary>UI theme (Light, Dark, or System).</summary>
    /// <example>Dark</example>
    public Theme? Theme { get; set; }

    /// <summary>IANA time-zone identifier (e.g. "Asia/Kolkata", "America/New_York").</summary>
    /// <example>Asia/Kolkata</example>
    public string? TimeZone { get; set; }

    /// <summary>Whether to receive e-mail notifications.</summary>
    /// <example>true</example>
    public bool? EmailNotifications { get; set; }

    /// <summary>Whether to receive push notifications.</summary>
    /// <example>true</example>
    public bool? PushNotifications { get; set; }

    /// <summary>Whether to receive SMS notifications.</summary>
    /// <example>false</example>
    public bool? SmsNotifications { get; set; }

    /// <summary>Whether to receive marketing e-mails.</summary>
    /// <example>false</example>
    public bool? MarketingEmails { get; set; }

    /// <summary>Whether the profile is visible to other users.</summary>
    /// <example>true</example>
    public bool? ProfileVisibility { get; set; }

    /// <summary>Whether to show online status to other users.</summary>
    /// <example>true</example>
    public bool? ShowOnlineStatus { get; set; }
}
