using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

using SportsGurukul.Application.Features.NotificationManagement.BusinessRules.Rules;

namespace SportsGurukul.Application.Features.NotificationManagement.BusinessRules;

public class RateLimitRule : IBusinessRule
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly ILogger<RateLimitRule> _logger;

    public RateLimitRule(
        INotificationRepository notificationRepository,
        IPreferenceRepository preferenceRepository,
        ILogger<RateLimitRule> logger)
    {
        _notificationRepository = notificationRepository;
        _preferenceRepository = preferenceRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> ValidateAsync<T>(T request, CancellationToken cancellationToken = default)
    {
        if (request is not CreateNotificationRequest notificationRequest)
            return Result<bool>.Success(true);

        foreach (var recipient in notificationRequest.Recipients)
        {
            if (!recipient.UserId.HasValue)
                continue;

            var preference = await _preferenceRepository
                .GetByUserAndChannelAsync(
                    recipient.UserId.Value,
                    Domain.Enums.Notification.NotificationChannelType.Email,
                    cancellationToken);

            if (preference?.MaxPerDay is null)
                continue;

            var todayStart = DateTime.UtcNow.Date;
            var todayEnd = todayStart.AddDays(1);
            var todayNotifications = await _notificationRepository
                .FindAsync(n => n.CreatedAt >= todayStart && n.CreatedAt < todayEnd, cancellationToken);

            if (todayNotifications.Count >= preference.MaxPerDay.Value)
            {
                _logger.LogWarning("Rate limit exceeded for user {UserId}", recipient.UserId.Value);
                return Result<bool>.Failure($"Rate limit exceeded for user {recipient.UserId}");
            }
        }

        return Result<bool>.Success(true);
    }
}
