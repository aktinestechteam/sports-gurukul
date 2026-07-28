namespace SportsGurukul.Application.Features.EventSearchDiscovery.Engines;

public interface IPersonalizationService
{
    Task<UserPreferences> GetUserPreferencesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task UpdateUserPreferencesAsync(Guid userId, UserPreferences preferences, CancellationToken cancellationToken = default);
    Task TrackInteractionAsync(Guid userId, Guid eventId, string interactionType, CancellationToken cancellationToken = default);
}

public class UserPreferences
{
    public IReadOnlyList<string> PreferredSports { get; set; } = [];
    public IReadOnlyList<string> PreferredEventTypes { get; set; } = [];
    public string? PreferredCity { get; set; }
    public string? PreferredState { get; set; }
    public decimal? PreferredLatitude { get; set; }
    public decimal? PreferredLongitude { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxPrice { get; set; }
    public string? PreferredLanguage { get; set; }
    public string? SkillLevel { get; set; }
    public string? AgeGroup { get; set; }
}
