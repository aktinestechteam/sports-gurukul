using System.Collections.Concurrent;
using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class CampaignManagementService : ICampaignManagementService
{
    private readonly ILogger<CampaignManagementService> _logger;
    private readonly ICampaignService _campaignService;
    private readonly ISchedulingEngine _schedulingEngine;
    private readonly IAudienceSegmentationService _audienceSegmentationService;
    private readonly ICacheService _cache;
    private readonly IMediator _mediator;

    private readonly ConcurrentDictionary<Guid, CampaignDetailDto> _campaigns = new();
    private readonly ConcurrentDictionary<Guid, List<CampaignBatchDto>> _batches = new();
    private int _batchNumber;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public CampaignManagementService(
        ILogger<CampaignManagementService> logger,
        ICampaignService campaignService,
        ISchedulingEngine schedulingEngine,
        IAudienceSegmentationService audienceSegmentationService,
        ICacheService cache,
        IMediator mediator)
    {
        _logger = logger;
        _campaignService = campaignService;
        _schedulingEngine = schedulingEngine;
        _audienceSegmentationService = audienceSegmentationService;
        _cache = cache;
        _mediator = mediator;
    }

    public async Task<CampaignDetailDto> CreateAsync(CreateCampaignFullRequest request, string? createdBy, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new ArgumentException("Campaign name is required", nameof(request));

        if (request.Schedule != null)
        {
            var validation = await _schedulingEngine.ValidateScheduleAsync(request.Schedule, ct);
            if (!validation.IsValid)
                throw new InvalidOperationException(
                    $"Schedule validation failed: {string.Join("; ", validation.Errors)}");
        }

        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var dto = new CampaignDetailDto(
            id, request.Name, request.Description, request.CampaignType,
            CampaignStatus.Draft, request.TemplateId, null, request.ChannelType,
            request.Schedule, request.Audience, 0, 0, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 0.0, 0.0,
            null, null, null, null, createdBy, request.Metadata, now, new List<CampaignBatchDto>());

        _campaigns[id] = dto;

        if (request.Schedule != null)
        {
            try
            {
                var job = await _schedulingEngine.RegisterJobAsync(id, request.Schedule, ct);
                dto = dto with { ScheduledAt = job.NextRunAt };
                _campaigns[id] = dto;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to register schedule for campaign {CampaignId}", id);
            }
        }

        await _cache.SetAsync(CacheKeys.CampaignKey(id), dto, CacheDuration);

        try
        {
            var createReq = new Application.Features.NotificationManagement.DTOs.CreateCampaignRequest(
                request.Name, request.Description, request.TemplateId,
                (SportsGurukul.Domain.Enums.Notification.NotificationChannelType)(int)(object)request.ChannelType,
                dto.ScheduledAt, request.Audience?.CustomQuery, request.Metadata);
            await _campaignService.CreateAsync(createReq, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist campaign {CampaignId} via ICampaignService", id);
        }

        await _mediator.Publish(new CampaignCreatedEvent(id, request.Name, now), ct);
        _logger.LogInformation("Created campaign {CampaignId} '{Name}'", id, request.Name);
        return dto;
    }

    public async Task<CampaignDetailDto> UpdateAsync(Guid id, UpdateCampaignRequest request, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        var now = DateTime.UtcNow;

        var name = request.Name ?? existing.Name;
        var description = request.Description ?? existing.Description;
        var campaignType = request.CampaignType ?? existing.CampaignType;
        var templateId = request.TemplateId ?? existing.TemplateId;
        var channelType = request.ChannelType ?? existing.ChannelType;
        var schedule = request.Schedule ?? existing.Schedule;
        var audience = request.Audience ?? existing.Audience;
        var metadata = request.Metadata ?? existing.Metadata;

        var updated = existing with
        {
            Name = name,
            Description = description,
            CampaignType = campaignType,
            TemplateId = templateId,
            ChannelType = channelType,
            Schedule = schedule,
            Audience = audience,
            Metadata = metadata
        };

        _campaigns[id] = updated;

        if (request.Schedule != null)
        {
            try
            {
                await _schedulingEngine.UnregisterJobAsync(id, ct);
                var job = await _schedulingEngine.RegisterJobAsync(id, schedule!, ct);
                updated = updated with { ScheduledAt = job.NextRunAt };
                _campaigns[id] = updated;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to update schedule for campaign {CampaignId}", id);
            }
        }

        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);
        await _mediator.Publish(new CampaignUpdatedEvent(id, name, now), ct);
        _logger.LogInformation("Updated campaign {CampaignId} '{Name}'", id, name);
        return updated;
    }

    public async Task<CampaignDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cached = await _cache.GetAsync<CampaignDetailDto>(CacheKeys.CampaignKey(id));
        if (cached != null)
            return cached;

        if (_campaigns.TryGetValue(id, out var campaign))
        {
            await _cache.SetAsync(CacheKeys.CampaignKey(id), campaign, CacheDuration);
            return campaign;
        }

        try
        {
            var result = await _campaignService.GetByIdAsync(id, ct);
            if (result.IsSuccess && result.Value != null)
            {
                var d = result.Value;
                var dto = new CampaignDetailDto(
                    d.Id, d.Name, d.Description, CampaignType.Scheduled,
                    MapStatus(d.Status), d.TemplateId, null, d.ChannelType,
                    null, null, d.TotalCount, d.SuccessCount, d.SuccessCount,
                    d.FailureCount, 0, 0, 0, 0, 0.0, 0.0, 0.0, 0.0,
                    d.ScheduledAt, d.StartedAt, d.CompletedAt, null, null,
                    d.Metadata, d.CreatedAt, new List<CampaignBatchDto>());
                _campaigns[id] = dto;
                await _cache.SetAsync(CacheKeys.CampaignKey(id), dto, CacheDuration);
                return dto;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load campaign {CampaignId} from ICampaignService", id);
        }

        throw new KeyNotFoundException($"Campaign {id} not found");
    }

    public Task<CampaignSearchResult> SearchAsync(CampaignSearchCriteria criteria, CancellationToken ct = default)
    {
        var query = _campaigns.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            var q = criteria.Query.ToLowerInvariant();
            query = query.Where(c =>
                c.Name.ToLowerInvariant().Contains(q) ||
                (c.Description?.ToLowerInvariant().Contains(q) ?? false));
        }

        if (criteria.CampaignType.HasValue)
            query = query.Where(c => c.CampaignType == criteria.CampaignType.Value);

        if (criteria.Status.HasValue)
            query = query.Where(c => c.Status == criteria.Status.Value);

        if (criteria.ChannelType.HasValue)
            query = query.Where(c => c.ChannelType == criteria.ChannelType.Value);

        if (criteria.CreatedAfter.HasValue)
            query = query.Where(c => c.CreatedAt >= criteria.CreatedAfter.Value);

        if (criteria.CreatedBefore.HasValue)
            query = query.Where(c => c.CreatedAt <= criteria.CreatedBefore.Value);

        if (!string.IsNullOrWhiteSpace(criteria.CreatedBy))
            query = query.Where(c => c.CreatedBy == criteria.CreatedBy);

        if (criteria.HasSchedule.HasValue)
            query = query.Where(c => (c.Schedule != null) == criteria.HasSchedule.Value);

        if (criteria.HasAudience.HasValue)
            query = query.Where(c => (c.Audience != null) == criteria.HasAudience.Value);

        var totalCount = query.Count();
        var pageNumber = Math.Max(1, criteria.PageNumber);
        var pageSize = Math.Clamp(criteria.PageSize, 1, 100);
        var items = query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new CampaignSearchResult(
            items, totalCount, pageNumber, pageSize,
            (pageNumber * pageSize) < totalCount);

        return Task.FromResult(result);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _campaigns.TryRemove(id, out _);
        _batches.TryRemove(id, out _);
        await _cache.RemoveAsync(CacheKeys.CampaignKey(id));

        try
        {
            await _schedulingEngine.UnregisterJobAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to unregister schedule for deleted campaign {CampaignId}", id);
        }

        await _mediator.Publish(new CampaignDeletedEvent(id, DateTime.UtcNow), ct);
        _logger.LogInformation("Deleted campaign {CampaignId}", id);
        return true;
    }

    public async Task<CampaignDetailDto> ActivateAsync(Guid id, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        if (existing.Status != CampaignStatus.Draft)
            throw new InvalidOperationException($"Cannot activate campaign {id} from status {existing.Status}");

        var now = DateTime.UtcNow;
        var updated = existing with
        {
            Status = CampaignStatus.Active,
            StartedAt = now
        };

        _campaigns[id] = updated;

        if (updated.Schedule != null)
        {
            try
            {
                var job = await _schedulingEngine.RegisterJobAsync(id, updated.Schedule, ct);
                updated = updated with { ScheduledAt = job.NextRunAt };
                _campaigns[id] = updated;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to register schedule on activate for campaign {CampaignId}", id);
            }
        }

        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);
        await _mediator.Publish(new CampaignStatusChangedEvent(id, CampaignStatus.Draft, CampaignStatus.Active, now), ct);
        _logger.LogInformation("Activated campaign {CampaignId}", id);
        return updated;
    }

    public async Task<PauseCampaignResult> PauseAsync(Guid id, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        if (existing.Status != CampaignStatus.Active)
            throw new InvalidOperationException($"Cannot pause campaign {id} from status {existing.Status}");

        var now = DateTime.UtcNow;
        var previousStatus = existing.Status;

        var updated = existing with
        {
            Status = CampaignStatus.Paused
        };

        _campaigns[id] = updated;

        try
        {
            await _schedulingEngine.UnregisterJobAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to unregister schedule on pause for campaign {CampaignId}", id);
        }

        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);
        await _mediator.Publish(new CampaignStatusChangedEvent(id, previousStatus, CampaignStatus.Paused, now), ct);
        _logger.LogInformation("Paused campaign {CampaignId}", id);

        return new PauseCampaignResult(id, previousStatus, CampaignStatus.Paused, now);
    }

    public async Task<ResumeCampaignResult> ResumeAsync(Guid id, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        if (existing.Status != CampaignStatus.Paused)
            throw new InvalidOperationException($"Cannot resume campaign {id} from status {existing.Status}");

        var now = DateTime.UtcNow;
        var previousStatus = existing.Status;

        var updated = existing with
        {
            Status = CampaignStatus.Active
        };

        _campaigns[id] = updated;

        if (updated.Schedule != null)
        {
            try
            {
                var job = await _schedulingEngine.RegisterJobAsync(id, updated.Schedule, ct);
                updated = updated with { ScheduledAt = job.NextRunAt };
                _campaigns[id] = updated;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to register schedule on resume for campaign {CampaignId}", id);
            }
        }

        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);
        await _mediator.Publish(new CampaignStatusChangedEvent(id, previousStatus, CampaignStatus.Active, now), ct);
        _logger.LogInformation("Resumed campaign {CampaignId}", id);

        return new ResumeCampaignResult(id, previousStatus, CampaignStatus.Active, now);
    }

    public async Task<CampaignDetailDto> CancelAsync(Guid id, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        if (existing.Status is CampaignStatus.Cancelled or CampaignStatus.Completed)
            throw new InvalidOperationException($"Cannot cancel campaign {id} from status {existing.Status}");

        var now = DateTime.UtcNow;
        var previousStatus = existing.Status;

        var updated = existing with
        {
            Status = CampaignStatus.Cancelled,
            CompletedAt = now
        };

        _campaigns[id] = updated;

        try
        {
            await _schedulingEngine.UnregisterJobAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to unregister schedule on cancel for campaign {CampaignId}", id);
        }

        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);
        await _mediator.Publish(new CampaignStatusChangedEvent(id, previousStatus, CampaignStatus.Cancelled, now), ct);
        _logger.LogInformation("Cancelled campaign {CampaignId}", id);
        return updated;
    }

    public async Task<CampaignDetailDto> CloneAsync(Guid id, CampaignCloneRequest request, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var source))
            throw new KeyNotFoundException($"Source campaign {id} not found");

        var newId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var cloned = new CampaignDetailDto(
            newId,
            request.NewName,
            request.NewDescription ?? source.Description,
            source.CampaignType,
            CampaignStatus.Draft,
            request.IncludeTemplate ? source.TemplateId : null,
            request.IncludeTemplate ? source.TemplateName : null,
            source.ChannelType,
            request.IncludeSchedule ? source.Schedule : null,
            request.IncludeAudience ? source.Audience : null,
            0, 0, 0, 0, 0, 0, 0, 0, 0.0, 0.0, 0.0, 0.0,
            null, null, null, null, null, source.Metadata, now,
            new List<CampaignBatchDto>());

        _campaigns[newId] = cloned;

        if (cloned.Schedule != null)
        {
            try
            {
                var job = await _schedulingEngine.RegisterJobAsync(newId, cloned.Schedule, ct);
                cloned = cloned with { ScheduledAt = job.NextRunAt };
                _campaigns[newId] = cloned;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to register schedule for cloned campaign {CampaignId}", newId);
            }
        }

        await _cache.SetAsync(CacheKeys.CampaignKey(newId), cloned, CacheDuration);
        await _mediator.Publish(new CampaignCreatedEvent(newId, request.NewName, now), ct);
        _logger.LogInformation("Cloned campaign {SourceId} -> {TargetId} as '{Name}'", id, newId, request.NewName);
        return cloned;
    }

    public async Task<CampaignTriggerResult> TriggerNowAsync(Guid id, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        if (existing.Status is CampaignStatus.Cancelled or CampaignStatus.Completed or CampaignStatus.Archived)
            throw new InvalidOperationException($"Cannot trigger campaign {id} from status {existing.Status}");

        var now = DateTime.UtcNow;
        var batchNumber = Interlocked.Increment(ref _batchNumber);
        var batchId = Guid.NewGuid();

        var resolvedAudience = await ResolveAudienceAsync(existing.Audience, ct);
        var recipientCount = resolvedAudience.Count;

        var batch = new CampaignBatchDto(
            batchId, batchNumber, recipientCount, 0, 0, 0, now, null, null);

        _batches.AddOrUpdate(id,
            _ => new List<CampaignBatchDto> { batch },
            (_, list) => { list.Add(batch); return list; });

        var updated = existing with
        {
            TotalRecipients = recipientCount,
            SentCount = recipientCount,
            StartedAt = existing.StartedAt ?? now,
            LastProcessedAt = now,
            Batches = existing.Batches.Append(batch).ToList()
        };

        updated = RecalculateRates(updated);
        _campaigns[id] = updated;
        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);

        await _mediator.Publish(new CampaignTriggeredEvent(id, "Immediate", recipientCount, now), ct);
        _logger.LogInformation("Triggered campaign {CampaignId} immediately with {Count} recipients", id, recipientCount);

        return new CampaignTriggerResult(id, recipientCount, "Queued", now);
    }

    public async Task<CampaignTriggerResult> TriggerScheduledAsync(Guid id, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        if (existing.Status is CampaignStatus.Cancelled or CampaignStatus.Archived)
            throw new InvalidOperationException($"Cannot trigger scheduled campaign {id} from status {existing.Status}");

        if (existing.Schedule == null)
            throw new InvalidOperationException($"Campaign {id} has no schedule defined");

        var now = DateTime.UtcNow;
        var batchNumber = Interlocked.Increment(ref _batchNumber);
        var batchId = Guid.NewGuid();

        var resolvedAudience = await ResolveAudienceAsync(existing.Audience, ct);
        var recipientCount = resolvedAudience.Count;

        var batch = new CampaignBatchDto(
            batchId, batchNumber, recipientCount, 0, 0, 0, now, null, null);

        _batches.AddOrUpdate(id,
            _ => new List<CampaignBatchDto> { batch },
            (_, list) => { list.Add(batch); return list; });

        var nextRun = await _schedulingEngine.CalculateNextRunAsync(existing.Schedule, ct);
        var completed = existing.Schedule.EndDate.HasValue && now >= existing.Schedule.EndDate.Value;

        var updated = existing with
        {
            TotalRecipients = recipientCount,
            SentCount = recipientCount,
            Status = completed ? CampaignStatus.Completed : existing.Status,
            StartedAt = existing.StartedAt ?? now,
            CompletedAt = completed ? now : null,
            LastProcessedAt = now,
            ScheduledAt = nextRun,
            Batches = existing.Batches.Append(batch).ToList()
        };

        updated = RecalculateRates(updated);
        _campaigns[id] = updated;
        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);

        var status = completed ? "Completed" : "Scheduled";
        await _mediator.Publish(new CampaignTriggeredEvent(id, status, recipientCount, now), ct);
        _logger.LogInformation("Scheduled trigger for campaign {CampaignId} with {Count} recipients", id, recipientCount);

        return new CampaignTriggerResult(id, recipientCount, status, now);
    }

    public async Task<CampaignBulkCreateResult> BulkCreateAsync(CampaignBulkCreateRequest request, string? createdBy, CancellationToken ct = default)
    {
        var errors = new List<string>();
        var created = 0;
        var total = request.Campaigns.Count;

        for (int i = 0; i < total; i++)
        {
            var campaignRequest = request.Campaigns[i];
            try
            {
                if (string.IsNullOrWhiteSpace(campaignRequest.Name))
                {
                    errors.Add($"Campaign at index {i}: Name is required");
                    continue;
                }

                if (request.ValidateOnly)
                    continue;

                await CreateAsync(campaignRequest, createdBy, ct);
                created++;
            }
            catch (Exception ex)
            {
                errors.Add($"Campaign at index {i}: {ex.Message}");
                _logger.LogWarning(ex, "Failed to create campaign at index {Index} in bulk create", i);
            }
        }

        return new CampaignBulkCreateResult(total, created, total - created, errors);
    }

    public Task<int> GetCountByStatusAsync(CampaignStatus status, CancellationToken ct = default)
    {
        var count = _campaigns.Values.Count(c => c.Status == status);
        return Task.FromResult(count);
    }

    public Task<List<CampaignDetailDto>> GetDueCampaignsAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var due = _campaigns.Values
            .Where(c => c.Status == CampaignStatus.Active && c.Schedule != null)
            .Where(c => c.ScheduledAt.HasValue && c.ScheduledAt.Value <= now)
            .OrderBy(c => c.ScheduledAt)
            .ToList();

        return Task.FromResult(due);
    }

    public async Task<CampaignDetailDto> UpdateScheduleAsync(Guid id, ScheduleDefinitionDto schedule, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        var validation = await _schedulingEngine.ValidateScheduleAsync(schedule, ct);
        if (!validation.IsValid)
            throw new InvalidOperationException(
                $"Schedule validation failed: {string.Join("; ", validation.Errors)}");

        var updated = existing with { Schedule = schedule };

        try
        {
            await _schedulingEngine.UnregisterJobAsync(id, ct);
        }
        catch
        {
        }

        var nextRun = await _schedulingEngine.CalculateNextRunAsync(schedule, ct);
        updated = updated with { ScheduledAt = nextRun };

        try
        {
            var job = await _schedulingEngine.RegisterJobAsync(id, schedule, ct);
            updated = updated with { ScheduledAt = job.NextRunAt };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to register updated schedule for campaign {CampaignId}", id);
        }

        _campaigns[id] = updated;
        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);
        _logger.LogInformation("Updated schedule for campaign {CampaignId}", id);
        return updated;
    }

    public async Task<CampaignDetailDto> UpdateAudienceAsync(Guid id, AudienceDefinitionDto audience, CancellationToken ct = default)
    {
        if (!_campaigns.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Campaign {id} not found");

        var updated = existing with { Audience = audience };
        _campaigns[id] = updated;
        await _cache.SetAsync(CacheKeys.CampaignKey(id), updated, CacheDuration);
        _logger.LogInformation("Updated audience for campaign {CampaignId}", id);
        return updated;
    }

    private async Task<List<string>> ResolveAudienceAsync(AudienceDefinitionDto? audience, CancellationToken ct)
    {
        if (audience == null)
            return new List<string>();

        if (audience.IncludeAllUsers)
        {
            var result = await _audienceSegmentationService.ResolveSegmentAsync(SegmentType.AllUsers, null, ct);
            return result.UserIds;
        }

        var userIds = new HashSet<string>();

        if (audience.UserIds?.Count > 0)
        {
            foreach (var uid in audience.UserIds)
                userIds.Add(uid);
        }

        if (audience.SegmentIds?.Count > 0)
        {
            foreach (var segId in audience.SegmentIds)
            {
                if (Guid.TryParse(segId, out var guid))
                {
                    try
                    {
                        var result = await _audienceSegmentationService.EvaluateSegmentAsync(guid, ct);
                        foreach (var uid in result.UserIds)
                            userIds.Add(uid);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Failed to evaluate segment {SegmentId}", segId);
                    }
                }
            }
        }

        if (audience.RoleNames?.Count > 0)
        {
            foreach (var role in audience.RoleNames)
            {
                try
                {
                    var result = await _audienceSegmentationService.ResolveSegmentAsync(
                        SegmentType.ByRole, new Dictionary<string, object> { ["role"] = role }, ct);
                    foreach (var uid in result.UserIds)
                        userIds.Add(uid);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to resolve role segment '{Role}'", role);
                }
            }
        }

        if (audience.TagFilters?.Count > 0)
        {
            try
            {
                var result = await _audienceSegmentationService.ResolveSegmentAsync(
                    SegmentType.ByTag,
                    new Dictionary<string, object> { ["tags"] = string.Join(",", audience.TagFilters) }, ct);
                foreach (var uid in result.UserIds)
                    userIds.Add(uid);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve tag segment");
            }
        }

        if (audience.DynamicFilters?.Count > 0)
        {
            try
            {
                var result = await _audienceSegmentationService.ResolveSegmentAsync(
                    SegmentType.CustomDynamic, audience.DynamicFilters, ct);
                foreach (var uid in result.UserIds)
                    userIds.Add(uid);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to resolve dynamic segment");
            }
        }

        return userIds.ToList();
    }

    private static CampaignDetailDto RecalculateRates(CampaignDetailDto dto)
    {
        var total = dto.TotalRecipients;
        var delivered = dto.DeliveredCount;
        var opened = dto.OpenCount;
        var clicked = dto.ClickCount;
        var failed = dto.FailedCount;

        return dto with
        {
            DeliveryRate = total > 0 ? Math.Round((double)delivered / total * 100, 2) : 0.0,
            OpenRate = delivered > 0 ? Math.Round((double)opened / delivered * 100, 2) : 0.0,
            ClickRate = delivered > 0 ? Math.Round((double)clicked / delivered * 100, 2) : 0.0,
            FailureRate = total > 0 ? Math.Round((double)failed / total * 100, 2) : 0.0
        };
    }

    private static CampaignStatus MapStatus(SportsGurukul.Domain.Enums.Notification.NotificationStatus status)
    {
        return status switch
        {
            SportsGurukul.Domain.Enums.Notification.NotificationStatus.Draft => CampaignStatus.Draft,
            SportsGurukul.Domain.Enums.Notification.NotificationStatus.Sent => CampaignStatus.Active,
            SportsGurukul.Domain.Enums.Notification.NotificationStatus.Scheduled => CampaignStatus.Active,
            SportsGurukul.Domain.Enums.Notification.NotificationStatus.Failed => CampaignStatus.Completed,
            _ => CampaignStatus.Draft
        };
    }

    private static class CacheKeys
    {
        private const string CampaignPrefix = "campaigns:";
        public static string CampaignKey(Guid id) => $"{CampaignPrefix}{id}";
    }
}

public record CampaignCreatedEvent(Guid CampaignId, string Name, DateTime CreatedAt) : INotification;
public record CampaignUpdatedEvent(Guid CampaignId, string Name, DateTime UpdatedAt) : INotification;
public record CampaignDeletedEvent(Guid CampaignId, DateTime DeletedAt) : INotification;
public record CampaignStatusChangedEvent(Guid CampaignId, CampaignStatus PreviousStatus, CampaignStatus NewStatus, DateTime Timestamp) : INotification;
public record CampaignTriggeredEvent(Guid CampaignId, string Status, int RecipientsQueued, DateTime TriggeredAt) : INotification;
