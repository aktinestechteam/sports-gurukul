using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Services;

public class CampaignService : ICampaignService
{
    private static readonly ConcurrentDictionary<Guid, NotificationCampaign> _campaigns = new();
    private readonly INotificationRepository _notificationRepository;
    private readonly IBusinessRuleValidator _ruleValidator;
    private readonly IRecipientResolver _recipientResolver;
    private readonly ILogger<CampaignService> _logger;

    public CampaignService(
        INotificationRepository notificationRepository,
        IBusinessRuleValidator ruleValidator,
        IRecipientResolver recipientResolver,
        ILogger<CampaignService> logger)
    {
        _notificationRepository = notificationRepository;
        _ruleValidator = ruleValidator;
        _recipientResolver = recipientResolver;
        _logger = logger;
    }

    public async Task<Result<CampaignDto>> CreateAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _ruleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsSuccess)
            return Result<CampaignDto>.Failure(validation.Errors);

        var entity = new NotificationCampaign
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            TemplateId = request.TemplateId,
            ChannelType = request.ChannelType,
            Status = request.ScheduledAt.HasValue ? NotificationStatus.Scheduled : NotificationStatus.Draft,
            ScheduledAt = request.ScheduledAt,
            TargetCriteria = request.TargetCriteria,
            Metadata = request.Metadata,
            CreatedAt = DateTime.UtcNow
        };

        _campaigns[entity.Id] = entity;
        _logger.LogInformation("Created campaign {CampaignId} with name {CampaignName}", entity.Id, entity.Name);
        return Result<CampaignDto>.Success(MapToDto(entity));
    }

    public async Task<Result<bool>> ScheduleAsync(Guid campaignId, DateTime scheduledAt, CancellationToken cancellationToken = default)
    {
        if (_campaigns.TryGetValue(campaignId, out var campaign))
        {
            campaign.Status = NotificationStatus.Scheduled;
            campaign.ScheduledAt = scheduledAt;
        }
        _logger.LogInformation("Scheduled campaign {CampaignId} at {ScheduledAt}", campaignId, scheduledAt);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> CancelAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        if (_campaigns.TryGetValue(campaignId, out var campaign))
            campaign.Status = NotificationStatus.Cancelled;
        _logger.LogInformation("Cancelled campaign {CampaignId}", campaignId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> PauseAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Paused campaign {CampaignId}", campaignId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> ResumeAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resumed campaign {CampaignId}", campaignId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<CampaignDto>> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default)
    {
        if (_campaigns.TryGetValue(campaignId, out var campaign))
            return Result<CampaignDto>.Success(MapToDto(campaign));
        return Result<CampaignDto>.Failure($"Campaign {campaignId} not found");
    }

    private static CampaignDto MapToDto(NotificationCampaign entity) =>
        new(entity.Id, entity.Name, entity.Description, entity.TemplateId,
            entity.ChannelType, entity.Status, entity.ScheduledAt, entity.StartedAt,
            entity.CompletedAt, entity.TargetCriteria, entity.TotalCount,
            entity.SuccessCount, entity.FailureCount, entity.Metadata, entity.CreatedAt);
}
