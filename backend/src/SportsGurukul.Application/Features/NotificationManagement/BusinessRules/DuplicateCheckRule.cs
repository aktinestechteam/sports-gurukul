using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

using SportsGurukul.Application.Features.NotificationManagement.BusinessRules.Rules;

namespace SportsGurukul.Application.Features.NotificationManagement.BusinessRules;

public class DuplicateCheckRule : IBusinessRule
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ILogger<DuplicateCheckRule> _logger;

    public DuplicateCheckRule(
        INotificationRepository notificationRepository,
        ILogger<DuplicateCheckRule> logger)
    {
        _notificationRepository = notificationRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> ValidateAsync<T>(T request, CancellationToken cancellationToken = default)
    {
        if (request is not CreateNotificationRequest notificationRequest)
            return Result<bool>.Success(true);

        if (string.IsNullOrEmpty(notificationRequest.ExternalId) && notificationRequest.BatchId is null)
            return Result<bool>.Success(true);

        if (!string.IsNullOrEmpty(notificationRequest.ExternalId))
        {
            var existing = await _notificationRepository
                .FindAsync(n => n.ExternalId == notificationRequest.ExternalId, cancellationToken);
            if (existing.Count > 0)
            {
                _logger.LogWarning("Duplicate notification detected with ExternalId {ExternalId}",
                    notificationRequest.ExternalId);
                return Result<bool>.Failure($"Notification with ExternalId '{notificationRequest.ExternalId}' already exists");
            }
        }

        return Result<bool>.Success(true);
    }
}
