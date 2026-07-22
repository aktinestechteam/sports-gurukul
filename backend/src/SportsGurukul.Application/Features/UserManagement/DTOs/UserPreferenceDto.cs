using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.DTOs;

public class UserPreferenceDto
{
    public Guid Id { get; set; }
    public string Language { get; set; } = "en";
    public Theme Theme { get; set; } = Theme.System;
    public string TimeZone { get; set; } = "UTC";
    public bool EmailNotifications { get; set; } = true;
    public bool PushNotifications { get; set; } = true;
    public bool SmsNotifications { get; set; }
    public bool MarketingEmails { get; set; }
    public bool ProfileVisibility { get; set; } = true;
    public bool ShowOnlineStatus { get; set; } = true;
}
