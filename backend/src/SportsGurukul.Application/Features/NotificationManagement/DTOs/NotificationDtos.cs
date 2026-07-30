using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.DTOs;

public record NotificationDto(
    Guid Id,
    Guid? TemplateId,
    Guid ChannelId,
    string ChannelName,
    Guid? ProviderId,
    string? ProviderName,
    NotificationPriority Priority,
    NotificationStatus Status,
    string Subject,
    string Body,
    string? SenderId,
    DateTime? ScheduledAt,
    DateTime? SentAt,
    DateTime? DeliveredAt,
    DateTime? ReadAt,
    string? FailureReason,
    string? ErrorCode,
    Guid? BatchId,
    Guid? CampaignId,
    string? ExternalId,
    string? Metadata,
    DateTime CreatedAt,
    List<NotificationRecipientDto> Recipients,
    List<NotificationAttachmentDto> Attachments
);

public record NotificationSummaryDto(
    Guid Id,
    NotificationPriority Priority,
    NotificationStatus Status,
    string Subject,
    string ChannelName,
    int RecipientCount,
    DateTime? SentAt,
    DateTime CreatedAt
);

public record NotificationRecipientDto(
    Guid Id,
    Guid? UserId,
    string ChannelType,
    string DestinationAddress,
    string? RecipientName,
    NotificationStatus Status,
    DateTime? SentAt,
    DateTime? DeliveredAt,
    DateTime? ReadAt,
    string? FailureReason
);

public record NotificationAttachmentDto(
    Guid Id,
    string FileName,
    string ContentType,
    long FileSize,
    string StorageType
);

public record CreateNotificationRequest(
    Guid? TemplateId,
    Guid ChannelId,
    Guid? ProviderId,
    NotificationPriority Priority,
    string Subject,
    string Body,
    string? SenderId,
    DateTime? ScheduledAt,
    Guid? BatchId,
    Guid? CampaignId,
    string? ExternalId,
    string? Metadata,
    List<CreateRecipientRequest> Recipients,
    List<CreateAttachmentRequest>? Attachments
);

public record UpdateNotificationRequest(
    Guid Id,
    string? Subject,
    string? Body,
    NotificationPriority? Priority,
    Guid? ProviderId,
    DateTime? ScheduledAt,
    string? Metadata
);

public record CreateRecipientRequest(
    Guid? UserId,
    string ChannelType,
    string DestinationAddress,
    string? RecipientName
);

public record CreateAttachmentRequest(
    string FileName,
    string FilePath,
    string ContentType,
    long FileSize,
    string StorageType,
    Guid? DocumentId
);
