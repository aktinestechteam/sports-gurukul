using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.DTOs;

public record DeliveryDto(
    Guid Id,
    Guid NotificationId,
    Guid? RecipientId,
    Guid? ProviderId,
    string? ProviderName,
    NotificationChannelType ChannelType,
    NotificationStatus Status,
    DateTime? SentAt,
    DateTime? DeliveredAt,
    DateTime? ReadAt,
    string? FailureReason,
    string? ProviderMessageId,
    int AttemptCount,
    long? DurationMs,
    List<DeliveryRetryDto> Retries
);

public record DeliveryRetryDto(
    Guid Id,
    int AttemptNumber,
    DateTime AttemptedAt,
    NotificationStatus Status,
    string? FailureReason,
    bool IsFinal
);
