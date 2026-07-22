using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.Commands.UpdateUserPreference;

public class UpdateUserPreferenceCommand : IRequest<Result<UserPreferenceDto>>
{
    public Guid UserId { get; set; }
    public string? Language { get; set; }
    public Theme? Theme { get; set; }
    public string? TimeZone { get; set; }
    public bool? EmailNotifications { get; set; }
    public bool? PushNotifications { get; set; }
    public bool? SmsNotifications { get; set; }
    public bool? MarketingEmails { get; set; }
    public bool? ProfileVisibility { get; set; }
    public bool? ShowOnlineStatus { get; set; }
}
