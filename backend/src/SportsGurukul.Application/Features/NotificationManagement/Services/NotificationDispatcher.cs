using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Services;

public class NotificationDispatcher : INotificationDispatcher
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly IRecipientResolver _recipientResolver;
    private readonly ILogger<NotificationDispatcher> _logger;

    public NotificationDispatcher(
        INotificationRepository notificationRepository,
        IDeliveryRepository deliveryRepository,
        IPreferenceRepository preferenceRepository,
        ITemplateRenderer templateRenderer,
        IRecipientResolver recipientResolver,
        ILogger<NotificationDispatcher> logger)
    {
        _notificationRepository = notificationRepository;
        _deliveryRepository = deliveryRepository;
        _preferenceRepository = preferenceRepository;
        _templateRenderer = templateRenderer;
        _recipientResolver = recipientResolver;
        _logger = logger;
    }

    public async Task<Result<bool>> DispatchAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdWithDetailsAsync(notificationId, cancellationToken);
        if (notification is null)
            return Result<bool>.Failure($"Notification {notificationId} not found");

        _logger.LogInformation("Dispatching notification {NotificationId} to {RecipientCount} recipients",
            notificationId, notification.Recipients.Count);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DispatchToRecipientAsync(Guid notificationId, Guid recipientId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Dispatching notification {NotificationId} to recipient {RecipientId}",
            notificationId, recipientId);
        return Result<bool>.Success(true);
    }
}
