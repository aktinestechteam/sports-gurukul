using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Services;

public class PreferenceService : IPreferenceService
{
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IBusinessRuleValidator _ruleValidator;
    private readonly ILogger<PreferenceService> _logger;

    public PreferenceService(
        IPreferenceRepository preferenceRepository,
        INotificationRepository notificationRepository,
        IBusinessRuleValidator ruleValidator,
        ILogger<PreferenceService> logger)
    {
        _preferenceRepository = preferenceRepository;
        _notificationRepository = notificationRepository;
        _ruleValidator = ruleValidator;
        _logger = logger;
    }

    public async Task<Result<PreferenceDto>> CreateAsync(CreatePreferenceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _ruleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsSuccess)
            return Result<PreferenceDto>.Failure(validation.Errors);

        var existing = await _preferenceRepository
            .GetByUserAndChannelAsync(request.UserId, request.ChannelType, cancellationToken);
        if (existing is not null)
            return Result<PreferenceDto>.Failure($"Preference already exists for user {request.UserId} and channel {request.ChannelType}");

        var entity = new NotificationPreference
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ChannelType = request.ChannelType,
            IsEnabled = request.IsEnabled,
            QuietHoursStart = request.QuietHoursStart,
            QuietHoursEnd = request.QuietHoursEnd,
            MaxPerDay = request.MaxPerDay,
            CreatedAt = DateTime.UtcNow
        };

        await _preferenceRepository.AddAsync(entity, cancellationToken);
        _logger.LogInformation("Created preference {PreferenceId} for user {UserId}", entity.Id, entity.UserId);

        return Result<PreferenceDto>.Success(MapToDto(entity));
    }

    public async Task<Result<PreferenceDto>> UpdateAsync(UpdatePreferenceRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _ruleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsSuccess)
            return Result<PreferenceDto>.Failure(validation.Errors);

        var entity = await _preferenceRepository
            .GetByUserAndChannelAsync(request.UserId, request.ChannelType, cancellationToken);

        if (entity is null)
        {
            entity = new NotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                ChannelType = request.ChannelType,
                IsEnabled = request.IsEnabled ?? true,
                QuietHoursStart = request.QuietHoursStart,
                QuietHoursEnd = request.QuietHoursEnd,
                MaxPerDay = request.MaxPerDay,
                CreatedAt = DateTime.UtcNow
            };
            await _preferenceRepository.AddAsync(entity, cancellationToken);
            _logger.LogInformation("Created preference {PreferenceId} for user {UserId}", entity.Id, entity.UserId);
            return Result<PreferenceDto>.Success(MapToDto(entity));
        }

        if (request.IsEnabled.HasValue) entity.IsEnabled = request.IsEnabled.Value;
        if (request.QuietHoursStart is not null) entity.QuietHoursStart = request.QuietHoursStart;
        if (request.QuietHoursEnd is not null) entity.QuietHoursEnd = request.QuietHoursEnd;
        if (request.MaxPerDay.HasValue) entity.MaxPerDay = request.MaxPerDay;
        entity.UpdatedAt = DateTime.UtcNow;

        _preferenceRepository.Update(entity);
        _logger.LogInformation("Updated preference {PreferenceId} for user {UserId}", entity.Id, entity.UserId);

        return Result<PreferenceDto>.Success(MapToDto(entity));
    }

    public async Task<Result<List<PreferenceDto>>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var entities = await _preferenceRepository.GetByUserIdAsync(userId, cancellationToken);
        var dtos = entities.Select(MapToDto).ToList();
        return Result<List<PreferenceDto>>.Success(dtos);
    }

    public async Task<Result<bool>> IsChannelEnabledAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default)
    {
        var enabled = await _preferenceRepository.IsChannelEnabledAsync(userId, channelType, cancellationToken);
        return Result<bool>.Success(enabled);
    }

    public async Task<Result<bool>> MuteChannelAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default)
    {
        var entity = await _preferenceRepository
            .GetByUserAndChannelAsync(userId, channelType, cancellationToken);

        if (entity is null)
        {
            await _preferenceRepository.AddAsync(new NotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ChannelType = channelType,
                IsEnabled = false,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
            _logger.LogInformation("Created and muted channel {ChannelType} for user {UserId}", channelType, userId);
            return Result<bool>.Success(true);
        }

        entity.IsEnabled = false;
        entity.UpdatedAt = DateTime.UtcNow;
        _preferenceRepository.Update(entity);
        _logger.LogInformation("Muted channel {ChannelType} for user {UserId}", channelType, userId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UnmuteChannelAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default)
    {
        var entity = await _preferenceRepository
            .GetByUserAndChannelAsync(userId, channelType, cancellationToken);

        if (entity is null)
        {
            await _preferenceRepository.AddAsync(new NotificationPreference
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ChannelType = channelType,
                IsEnabled = true,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
            _logger.LogInformation("Created and unmuted channel {ChannelType} for user {UserId}", channelType, userId);
            return Result<bool>.Success(true);
        }

        entity.IsEnabled = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _preferenceRepository.Update(entity);
        _logger.LogInformation("Unmuted channel {ChannelType} for user {UserId}", channelType, userId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> SubscribeAsync(SubscribeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {UserId} subscribed to {EventType} for {EntityType}:{EntityId}",
            request.UserId, request.EventType, request.EntityType, request.EntityId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> UnsubscribeAsync(UnsubscribeRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("User {UserId} unsubscribed from {EventType} for {EntityType}:{EntityId}",
            request.UserId, request.EventType, request.EntityType, request.EntityId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> IsInQuietHoursAsync(Guid userId, NotificationChannelType channelType, CancellationToken cancellationToken = default)
    {
        var entity = await _preferenceRepository
            .GetByUserAndChannelAsync(userId, channelType, cancellationToken);

        if (entity is null || entity.QuietHoursStart is null || entity.QuietHoursEnd is null)
            return Result<bool>.Success(false);

        var now = TimeOnly.FromDateTime(DateTime.UtcNow);
        var inQuietHours = now >= entity.QuietHoursStart.Value && now <= entity.QuietHoursEnd.Value;
        return Result<bool>.Success(inQuietHours);
    }

    private static PreferenceDto MapToDto(NotificationPreference entity) =>
        new(entity.Id, entity.UserId, entity.ChannelType, entity.IsEnabled,
            entity.QuietHoursStart, entity.QuietHoursEnd, entity.MaxPerDay);
}
