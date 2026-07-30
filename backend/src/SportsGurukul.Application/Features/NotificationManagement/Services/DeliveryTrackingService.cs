using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Services;

public class DeliveryTrackingService : IDeliveryTrackingService
{
    private readonly IDeliveryRepository _deliveryRepository;
    private readonly ILogger<DeliveryTrackingService> _logger;

    public DeliveryTrackingService(
        IDeliveryRepository deliveryRepository,
        ILogger<DeliveryTrackingService> logger)
    {
        _deliveryRepository = deliveryRepository;
        _logger = logger;
    }

    public async Task<Result<List<DeliveryDto>>> GetByNotificationIdAsync(Guid notificationId, CancellationToken cancellationToken = default)
    {
        var deliveries = await _deliveryRepository.GetByNotificationIdAsync(notificationId, cancellationToken);
        var dtos = deliveries.Select(MapToDto).ToList();
        return Result<List<DeliveryDto>>.Success(dtos);
    }

    public async Task<Result<bool>> UpdateStatusAsync(Guid deliveryId, NotificationStatus status,
        string? providerMessageId = null, string? providerResponse = null,
        CancellationToken cancellationToken = default)
    {
        var entity = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Delivery {deliveryId} not found");

        entity.Status = status;
        if (providerMessageId is not null) entity.ProviderMessageId = providerMessageId;
        if (providerResponse is not null) entity.ProviderResponse = providerResponse;

        if (status == NotificationStatus.Sent) entity.SentAt = DateTime.UtcNow;
        if (status == NotificationStatus.Delivered) entity.DeliveredAt = DateTime.UtcNow;

        entity.UpdatedAt = DateTime.UtcNow;
        _deliveryRepository.Update(entity);
        _logger.LogInformation("Updated delivery {DeliveryId} status to {Status}", deliveryId, status);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RecordFailureAsync(Guid deliveryId, string failureReason, bool isFinal,
        CancellationToken cancellationToken = default)
    {
        var entity = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Delivery {deliveryId} not found");

        entity.Status = NotificationStatus.Failed;
        entity.FailureReason = failureReason;
        entity.FailedAt = DateTime.UtcNow;
        entity.AttemptCount++;
        entity.UpdatedAt = DateTime.UtcNow;
        _deliveryRepository.Update(entity);
        _logger.LogWarning("Delivery {DeliveryId} failed: {FailureReason} (isFinal={IsFinal})",
            deliveryId, failureReason, isFinal);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RecordReadAsync(Guid deliveryId, CancellationToken cancellationToken = default)
    {
        var entity = await _deliveryRepository.GetByIdAsync(deliveryId, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Delivery {deliveryId} not found");

        entity.Status = NotificationStatus.Read;
        entity.ReadAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _deliveryRepository.Update(entity);
        _logger.LogInformation("Delivery {DeliveryId} marked as read", deliveryId);
        return Result<bool>.Success(true);
    }

    private static DeliveryDto MapToDto(Domain.Entities.Notification.NotificationDelivery entity)
    {
        var retries = entity.Retries?
            .Select(r => new DeliveryRetryDto(
                r.Id, r.AttemptNumber, r.AttemptedAt, r.Status,
                r.FailureReason, r.IsFinal))
            .ToList() ?? [];

        return new DeliveryDto(
            entity.Id, entity.NotificationId, entity.RecipientId,
            entity.ProviderId, entity.Provider?.Name,
            entity.ChannelType, entity.Status, entity.SentAt,
            entity.DeliveredAt, entity.ReadAt, entity.FailureReason,
            entity.ProviderMessageId, entity.AttemptCount,
            entity.DurationMs, retries);
    }
}
