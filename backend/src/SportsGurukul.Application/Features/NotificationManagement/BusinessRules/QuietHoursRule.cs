using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

using SportsGurukul.Application.Features.NotificationManagement.BusinessRules.Rules;

namespace SportsGurukul.Application.Features.NotificationManagement.BusinessRules;

public class QuietHoursRule : IBusinessRule
{
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly ILogger<QuietHoursRule> _logger;

    public QuietHoursRule(
        IPreferenceRepository preferenceRepository,
        ILogger<QuietHoursRule> logger)
    {
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

            if (preference is null)
                continue;

            if (preference.QuietHoursStart is null || preference.QuietHoursEnd is null)
                continue;

            var now = TimeOnly.FromDateTime(DateTime.UtcNow);
            if (now >= preference.QuietHoursStart.Value && now <= preference.QuietHoursEnd.Value)
            {
                _logger.LogWarning("Recipient {UserId} is in quiet hours", recipient.UserId.Value);
                return Result<bool>.Failure($"Recipient {recipient.UserId} is in quiet hours");
            }
        }

        return Result<bool>.Success(true);
    }
}
