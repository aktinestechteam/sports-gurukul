using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IEventSearchRepository
{
    Task<IReadOnlyList<Event>> SearchEventsAsync(
        string? searchTerm, Guid? sportId, Guid? academyId, Guid? coachId,
        string? eventType, string? category, string? skillLevel, string? ageGroup,
        string? city, string? state, string? country,
        DateTime? dateFrom, DateTime? dateTo,
        decimal? minPrice, decimal? maxPrice,
        decimal? minRating, string? language,
        string? availability, string? registrationStatus,
        string? sortBy, bool sortDescending,
        int page, int pageSize, CancellationToken cancellationToken = default);

    Task<int> CountSearchEventsAsync(
        string? searchTerm, Guid? sportId, Guid? academyId, Guid? coachId,
        string? eventType, string? category, string? skillLevel, string? ageGroup,
        string? city, string? state, string? country,
        DateTime? dateFrom, DateTime? dateTo,
        decimal? minPrice, decimal? maxPrice,
        decimal? minRating, string? language,
        string? availability, string? registrationStatus,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Event>> GetNearbyEventsAsync(
        decimal latitude, decimal longitude, decimal radiusKm,
        int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<EventAutocompleteResult>> GetAutocompleteSuggestionsAsync(
        string prefix, int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Event>> GetTrendingEventsAsync(int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Event>> GetFeaturedEventsAsync(int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(int limit, DateTime? fromDate, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Event>> GetSimilarEventsAsync(Guid eventId, int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetPopularSearchTermsAsync(int limit, CancellationToken cancellationToken = default);

    Task<int> GetViewCountAsync(Guid eventId, CancellationToken cancellationToken = default);
    Task TrackViewAsync(Guid eventId, Guid? userId, string? source, string? deviceType, CancellationToken cancellationToken = default);

    Task SaveSearchAsync(EventSavedSearch savedSearch, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventSavedSearch>> GetSavedSearchesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteSavedSearchAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task RecordRecentSearchAsync(EventRecentSearch recentSearch, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<EventRecentSearch>> GetRecentSearchesAsync(Guid userId, int limit, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Event>> GetCalendarEventsAsync(DateTime fromDate, DateTime toDate, Guid? academyId, CancellationToken cancellationToken = default);
}

public class EventAutocompleteResult
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? SubText { get; set; }
    public string? EventType { get; set; }
    public DateTime? EventDate { get; set; }
}
