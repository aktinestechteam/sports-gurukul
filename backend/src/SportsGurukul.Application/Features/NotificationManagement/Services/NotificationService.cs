using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Services;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ITemplateRepository _templateRepository;
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly IBusinessRuleValidator _ruleValidator;
    private readonly IPublisher _publisher;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        ITemplateRepository templateRepository,
        IPreferenceRepository preferenceRepository,
        IBusinessRuleValidator ruleValidator,
        IPublisher publisher,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _templateRepository = templateRepository;
        _preferenceRepository = preferenceRepository;
        _ruleValidator = ruleValidator;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result<NotificationDto>> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _ruleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsSuccess)
            return Result<NotificationDto>.Failure(validation.Errors);

        var entity = new Domain.Entities.Notification.Notification
        {
            Id = Guid.NewGuid(),
            TemplateId = request.TemplateId,
            ChannelId = request.ChannelId,
            ProviderId = request.ProviderId,
            Priority = request.Priority,
            Status = NotificationStatus.Draft,
            Subject = request.Subject,
            Body = request.Body,
            SenderId = request.SenderId,
            ScheduledAt = request.ScheduledAt,
            BatchId = request.BatchId,
            CampaignId = request.CampaignId,
            ExternalId = request.ExternalId,
            Metadata = request.Metadata,
            CreatedAt = DateTime.UtcNow,
            Recipients = request.Recipients.Select(r => new NotificationRecipient
            {
                Id = Guid.NewGuid(),
                UserId = r.UserId,
                ChannelType = Enum.Parse<NotificationChannelType>(r.ChannelType),
                DestinationAddress = r.DestinationAddress,
                RecipientName = r.RecipientName,
                Status = NotificationStatus.Draft,
            }).ToList(),
            Attachments = request.Attachments?.Select(a => new NotificationAttachment
            {
                Id = Guid.NewGuid(),
                FileName = a.FileName,
                FilePath = a.FilePath,
                ContentType = a.ContentType,
                FileSize = a.FileSize,
                StorageType = a.StorageType,
                DocumentId = a.DocumentId,
            }).ToList() ?? []
        };

        await _notificationRepository.AddAsync(entity, cancellationToken);
        _logger.LogInformation("Created notification {NotificationId}", entity.Id);

        return Result<NotificationDto>.Success(MapToDto(entity));
    }

    public async Task<Result<NotificationDto>> UpdateAsync(UpdateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result<NotificationDto>.Failure($"Notification {request.Id} not found");

        var validation = await _ruleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsSuccess)
            return Result<NotificationDto>.Failure(validation.Errors);

        if (request.Subject is not null) entity.Subject = request.Subject;
        if (request.Body is not null) entity.Body = request.Body;
        if (request.Priority.HasValue) entity.Priority = request.Priority.Value;
        if (request.ProviderId.HasValue) entity.ProviderId = request.ProviderId;
        if (request.ScheduledAt.HasValue) entity.ScheduledAt = request.ScheduledAt;
        if (request.Metadata is not null) entity.Metadata = request.Metadata;
        entity.UpdatedAt = DateTime.UtcNow;

        _notificationRepository.Update(entity);
        _logger.LogInformation("Updated notification {NotificationId}", entity.Id);

        return Result<NotificationDto>.Success(MapToDto(entity));
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _notificationRepository.Update(entity);
        _logger.LogInformation("Deleted notification {NotificationId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> QueueAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        entity.Status = NotificationStatus.Queued;
        entity.UpdatedAt = DateTime.UtcNow;
        _notificationRepository.Update(entity);
        _logger.LogInformation("Queued notification {NotificationId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ScheduleAsync(Guid id, DateTime scheduledAt, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        entity.Status = NotificationStatus.Scheduled;
        entity.ScheduledAt = scheduledAt;
        entity.UpdatedAt = DateTime.UtcNow;
        _notificationRepository.Update(entity);
        _logger.LogInformation("Scheduled notification {NotificationId} at {ScheduledAt}", id, scheduledAt);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        if (entity.Status is NotificationStatus.Sent or NotificationStatus.Delivered)
            return Result<bool>.Failure("Cannot cancel a sent or delivered notification");

        entity.Status = NotificationStatus.Cancelled;
        entity.UpdatedAt = DateTime.UtcNow;
        _notificationRepository.Update(entity);
        _logger.LogInformation("Cancelled notification {NotificationId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SendAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        entity.Status = NotificationStatus.Sending;
        entity.SentAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;
        _notificationRepository.Update(entity);
        _logger.LogInformation("Sending notification {NotificationId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RetryAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        if (entity.Status is not (NotificationStatus.Failed or NotificationStatus.Sending))
            return Result<bool>.Failure("Only failed or sending notifications can be retried");

        entity.Status = NotificationStatus.Queued;
        entity.FailureReason = null;
        entity.ErrorCode = null;
        entity.UpdatedAt = DateTime.UtcNow;
        _notificationRepository.Update(entity);
        _logger.LogInformation("Retrying notification {NotificationId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ExpireAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        entity.Status = NotificationStatus.Expired;
        entity.UpdatedAt = DateTime.UtcNow;
        _notificationRepository.Update(entity);
        _logger.LogInformation("Expired notification {NotificationId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> MarkReadAsync(Guid id, Guid? userId, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        entity.Status = NotificationStatus.Read;
        entity.ReadAt = DateTime.UtcNow;
        entity.UpdatedAt = DateTime.UtcNow;

        if (userId.HasValue)
        {
            var recipient = entity.Recipients.FirstOrDefault(r => r.UserId == userId.Value);
            if (recipient is not null)
            {
                recipient.IsRead = true;
                recipient.ReadAtTimestamp = DateTime.UtcNow;
                recipient.ReadAt = DateTime.UtcNow;
                recipient.Status = NotificationStatus.Read;
            }
        }

        _notificationRepository.Update(entity);
        _logger.LogInformation("Marked notification {NotificationId} as read", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Notification {id} not found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _notificationRepository.Update(entity);
        _logger.LogInformation("Archived notification {NotificationId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<NotificationDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _notificationRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null)
            return Result<NotificationDto>.Failure($"Notification {id} not found");

        return Result<NotificationDto>.Success(MapToDto(entity));
    }

    private static NotificationDto MapToDto(Domain.Entities.Notification.Notification entity)
    {
        var recipients = entity.Recipients?.Select(r => new NotificationRecipientDto(
            r.Id, r.UserId, r.ChannelType.ToString(), r.DestinationAddress,
            r.RecipientName, r.Status, r.SentAt, r.DeliveredAt, r.ReadAt, r.FailureReason
        )).ToList() ?? [];

        return new NotificationDto(
            entity.Id, entity.TemplateId, entity.ChannelId,
            entity.Channel?.Name ?? string.Empty,
            entity.ProviderId, entity.Provider?.Name,
            entity.Priority, entity.Status, entity.Subject, entity.Body,
            entity.SenderId, entity.ScheduledAt, entity.SentAt,
            entity.DeliveredAt, entity.ReadAt, entity.FailureReason,
            entity.ErrorCode, entity.BatchId, entity.CampaignId,
            entity.ExternalId, entity.Metadata, entity.CreatedAt,
            recipients,
            entity.Attachments?.Select(a => new NotificationAttachmentDto(
                a.Id, a.FileName, a.ContentType, a.FileSize, a.StorageType
            )).ToList() ?? []
        );
    }
}
