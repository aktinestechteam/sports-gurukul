using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface IDeliveryTrackingService
{
    Task<Result<List<DeliveryDto>>> GetByNotificationIdAsync(Guid notificationId, CancellationToken cancellationToken = default);
    Task<Result<bool>> UpdateStatusAsync(Guid deliveryId, NotificationStatus status, string? providerMessageId = null, string? providerResponse = null, CancellationToken cancellationToken = default);
    Task<Result<bool>> RecordFailureAsync(Guid deliveryId, string failureReason, bool isFinal, CancellationToken cancellationToken = default);
    Task<Result<bool>> RecordReadAsync(Guid deliveryId, CancellationToken cancellationToken = default);
}
