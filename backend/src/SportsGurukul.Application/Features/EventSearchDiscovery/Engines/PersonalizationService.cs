using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

public class PersonalizationService : IPersonalizationService
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<PersonalizationService> _logger;

    private const string PreferenceCachePrefix = "user_preferences_";
    private const string InteractionCachePrefix = "user_interactions_";

    public PersonalizationService(ICacheService cacheService, ILogger<PersonalizationService> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<UserPreferences> GetUserPreferencesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{PreferenceCachePrefix}{userId}";
        var cached = await _cacheService.GetAsync<UserPreferences>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogInformation("Retrieved cached preferences for user {UserId}", userId);
            return cached;
        }

        _logger.LogInformation("No cached preferences for user {UserId}, returning defaults", userId);
        return new UserPreferences();
    }

    public async Task UpdateUserPreferencesAsync(Guid userId, UserPreferences preferences, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{PreferenceCachePrefix}{userId}";
        await _cacheService.SetAsync(cacheKey, preferences, TimeSpan.FromHours(24), cancellationToken);
        _logger.LogInformation("Updated preferences for user {UserId}", userId);
    }

    public async Task TrackInteractionAsync(Guid userId, Guid eventId, string interactionType, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"{InteractionCachePrefix}{userId}";
        var interactions = await _cacheService.GetAsync<List<UserInteraction>>(cacheKey, cancellationToken) ?? new List<UserInteraction>();

        interactions.Add(new UserInteraction
        {
            EventId = eventId,
            InteractionType = interactionType,
            InteractedAt = DateTime.UtcNow
        });

        if (interactions.Count > 100)
        {
            interactions = interactions[^100..];
        }

        await _cacheService.SetAsync(cacheKey, interactions, TimeSpan.FromDays(7), cancellationToken);
        _logger.LogInformation("Tracked interaction for user {UserId}: {InteractionType} on event {EventId}", userId, interactionType, eventId);
    }
}

public class UserInteraction
{
    public Guid EventId { get; set; }
    public string InteractionType { get; set; } = string.Empty;
    public DateTime InteractedAt { get; set; }
}
