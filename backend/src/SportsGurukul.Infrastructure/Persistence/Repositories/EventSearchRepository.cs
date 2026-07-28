using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class EventSearchRepository : IEventSearchRepository
{
    protected readonly ApplicationDbContext Context;

    public EventSearchRepository(ApplicationDbContext context)
    {
        Context = context;
    }

    public async Task<IReadOnlyList<Event>> SearchEventsAsync(
        string? searchTerm, Guid? sportId, Guid? academyId, Guid? coachId,
        string? eventType, string? category, string? skillLevel, string? ageGroup,
        string? city, string? state, string? country,
        DateTime? dateFrom, DateTime? dateTo,
        decimal? minPrice, decimal? maxPrice,
        decimal? minRating, string? language,
        string? availability, string? registrationStatus,
        string? sortBy, bool sortDescending,
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = BuildSearchQuery(searchTerm, sportId, academyId, coachId,
            eventType, category, skillLevel, ageGroup, city, state, country,
            dateFrom, dateTo, minPrice, maxPrice, minRating, language,
            availability, registrationStatus);

        query = ApplySorting(query, sortBy, sortDescending);

        return await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountSearchEventsAsync(
        string? searchTerm, Guid? sportId, Guid? academyId, Guid? coachId,
        string? eventType, string? category, string? skillLevel, string? ageGroup,
        string? city, string? state, string? country,
        DateTime? dateFrom, DateTime? dateTo,
        decimal? minPrice, decimal? maxPrice,
        decimal? minRating, string? language,
        string? availability, string? registrationStatus,
        CancellationToken cancellationToken = default)
    {
        var query = BuildSearchQuery(searchTerm, sportId, academyId, coachId,
            eventType, category, skillLevel, ageGroup, city, state, country,
            dateFrom, dateTo, minPrice, maxPrice, minRating, language,
            availability, registrationStatus);

        return await query.CountAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetNearbyEventsAsync(
        decimal latitude, decimal longitude, decimal radiusKm,
        int limit, CancellationToken cancellationToken = default)
    {
        var allEvents = await Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        e.Status != EventStatus.Draft &&
                        e.Status != EventStatus.Cancelled &&
                        e.Status != EventStatus.Archived)
            .Include(e => e.Venues.Where(v => !v.IsDeleted))
            .ToListAsync(cancellationToken);

        var nearby = allEvents
            .Where(e =>
            {
                var venue = e.Venues.FirstOrDefault(v => v.IsPrimary) ?? e.Venues.FirstOrDefault();
                if (venue?.Latitude == null || venue?.Longitude == null) return false;
                var distance = CalculateDistance(latitude, longitude, venue.Latitude.Value, venue.Longitude.Value);
                return distance <= (double)radiusKm;
            })
            .OrderBy(e =>
            {
                var venue = e.Venues.FirstOrDefault(v => v.IsPrimary) ?? e.Venues.FirstOrDefault();
                if (venue?.Latitude == null || venue?.Longitude == null) return double.MaxValue;
                return CalculateDistance(latitude, longitude, venue.Latitude.Value, venue.Longitude.Value);
            })
            .Take(limit)
            .ToList();

        return nearby;
    }

    public async Task<IReadOnlyList<EventAutocompleteResult>> GetAutocompleteSuggestionsAsync(
        string prefix, int limit, CancellationToken cancellationToken = default)
    {
        var term = prefix.ToLowerInvariant();

        var eventSuggestions = await Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        e.Status != EventStatus.Draft &&
                        (e.EventName.ToLower().Contains(term) ||
                         e.EventCode.ToLower().Contains(term) ||
                         (e.Tags != null && e.Tags.ToLower().Contains(term))))
            .OrderByDescending(e => e.IsFeatured)
            .ThenByDescending(e => e.StartDate)
            .Take(limit)
            .Select(e => new EventAutocompleteResult
            {
                Id = e.Id,
                Text = e.EventName,
                Type = "Event",
                SubText = e.EventCode,
                EventType = e.EventType != null ? e.EventType.Name : null,
                EventDate = e.StartDate
            })
            .ToListAsync(cancellationToken);

        return eventSuggestions;
    }

    public async Task<IReadOnlyList<Event>> GetTrendingEventsAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        e.Status != EventStatus.Draft &&
                        e.Status != EventStatus.Cancelled &&
                        e.Status != EventStatus.Archived &&
                        e.IsPublic)
            .Include(e => e.EventType)
            .Include(e => e.Academy)
            .Include(e => e.Sport)
            .Include(e => e.Venues.Where(v => !v.IsDeleted))
            .Include(e => e.Registrations.Where(r => !r.IsDeleted))
            .Include(e => e.Feedbacks.Where(f => !f.IsDeleted))
            .OrderByDescending(e => e.Registrations.Count(r => r.Status == EventRegistrationStatus.Approved))
            .ThenByDescending(e => e.StartDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetFeaturedEventsAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        e.IsFeatured &&
                        e.IsPublic &&
                        e.Status != EventStatus.Draft &&
                        e.Status != EventStatus.Cancelled &&
                        e.Status != EventStatus.Archived)
            .Include(e => e.EventType)
            .Include(e => e.Academy)
            .Include(e => e.Sport)
            .Include(e => e.Venues.Where(v => !v.IsDeleted))
            .Include(e => e.Registrations.Where(r => !r.IsDeleted))
            .Include(e => e.Feedbacks.Where(f => !f.IsDeleted))
            .OrderByDescending(e => e.StartDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetUpcomingEventsAsync(int limit, DateTime? fromDate, CancellationToken cancellationToken = default)
    {
        var effectiveDate = fromDate ?? DateTime.UtcNow;

        return await Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        e.StartDate >= effectiveDate &&
                        e.Status != EventStatus.Draft &&
                        e.Status != EventStatus.Cancelled &&
                        e.Status != EventStatus.Archived &&
                        e.IsPublic)
            .Include(e => e.EventType)
            .Include(e => e.Academy)
            .Include(e => e.Sport)
            .Include(e => e.Venues.Where(v => !v.IsDeleted))
            .Include(e => e.Feedbacks.Where(f => !f.IsDeleted))
            .OrderBy(e => e.StartDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetSimilarEventsAsync(Guid eventId, int limit, CancellationToken cancellationToken = default)
    {
        var sourceEvent = await Context.Events
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == eventId && !e.IsDeleted, cancellationToken);

        if (sourceEvent is null) return [];

        return await Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        e.Id != eventId &&
                        e.SportId == sourceEvent.SportId &&
                        e.Status != EventStatus.Draft &&
                        e.IsPublic)
            .Include(e => e.EventType)
            .Include(e => e.Academy)
            .Include(e => e.Sport)
            .Include(e => e.Venues.Where(v => !v.IsDeleted))
            .OrderByDescending(e => e.IsFeatured)
            .ThenByDescending(e => e.StartDate)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPopularSearchTermsAsync(int limit, CancellationToken cancellationToken = default)
    {
        return await Context.EventRecentSearches
            .AsNoTracking()
            .Where(s => !s.IsDeleted && !string.IsNullOrEmpty(s.SearchTerm))
            .GroupBy(s => s.SearchTerm.ToLower())
            .OrderByDescending(g => g.Count())
            .Take(limit)
            .Select(g => g.Key)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetViewCountAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        return await Context.EventRecentSearches
            .AsNoTracking()
            .CountAsync(s => !s.IsDeleted, cancellationToken);
    }

    public async Task TrackViewAsync(Guid eventId, Guid? userId, string? source, string? deviceType, CancellationToken cancellationToken = default)
    {
        var recentSearch = new EventRecentSearch
        {
            UserId = userId ?? Guid.Empty,
            SearchTerm = $"view:{eventId}",
            SearchedAt = DateTime.UtcNow,
            ResultCount = 1,
            CreatedAt = DateTime.UtcNow
        };

        Context.EventRecentSearches.Add(recentSearch);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task SaveSearchAsync(EventSavedSearch savedSearch, CancellationToken cancellationToken = default)
    {
        Context.EventSavedSearches.Add(savedSearch);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventSavedSearch>> GetSavedSearchesAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await Context.EventSavedSearches
            .AsNoTracking()
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteSavedSearchAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        var savedSearch = await Context.EventSavedSearches
            .FirstOrDefaultAsync(s => s.Id == id && s.UserId == userId && !s.IsDeleted, cancellationToken);

        if (savedSearch is null)
            throw new InvalidOperationException("Saved search not found or access denied.");

        savedSearch.IsDeleted = true;
        Context.EventSavedSearches.Update(savedSearch);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task RecordRecentSearchAsync(EventRecentSearch recentSearch, CancellationToken cancellationToken = default)
    {
        Context.EventRecentSearches.Add(recentSearch);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventRecentSearch>> GetRecentSearchesAsync(Guid userId, int limit, CancellationToken cancellationToken = default)
    {
        return await Context.EventRecentSearches
            .AsNoTracking()
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.SearchedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Event>> GetCalendarEventsAsync(DateTime fromDate, DateTime toDate, Guid? academyId, CancellationToken cancellationToken = default)
    {
        var query = Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted &&
                        e.StartDate >= fromDate &&
                        e.StartDate <= toDate &&
                        e.Status != EventStatus.Draft &&
                        e.Status != EventStatus.Archived)
            .Include(e => e.EventType)
            .Include(e => e.Academy)
            .Include(e => e.Sport)
            .Include(e => e.Venues.Where(v => !v.IsDeleted))
            .Include(e => e.Schedules.Where(s => !s.IsDeleted))
            .Include(e => e.Registrations.Where(r => !r.IsDeleted))
            .AsSplitQuery();

        if (academyId.HasValue)
            query = query.Where(e => e.AcademyId == academyId.Value);

        return await query
            .OrderBy(e => e.StartDate)
            .ToListAsync(cancellationToken);
    }

    private IQueryable<Event> BuildSearchQuery(
        string? searchTerm, Guid? sportId, Guid? academyId, Guid? coachId,
        string? eventType, string? category, string? skillLevel, string? ageGroup,
        string? city, string? state, string? country,
        DateTime? dateFrom, DateTime? dateTo,
        decimal? minPrice, decimal? maxPrice,
        decimal? minRating, string? language,
        string? availability, string? registrationStatus)
    {
        var query = Context.Events
            .AsNoTracking()
            .Where(e => !e.IsDeleted && e.IsPublic && e.Status != EventStatus.Draft)
            .Include(e => e.EventType)
            .Include(e => e.Academy)
            .Include(e => e.Sport)
            .Include(e => e.Venues.Where(v => !v.IsDeleted))
            .Include(e => e.Registrations.Where(r => !r.IsDeleted))
            .Include(e => e.Feedbacks.Where(f => !f.IsDeleted))
            .Include(e => e.Coaches.Where(c => !c.IsDeleted))
            .Include(e => e.Speakers.Where(s => !s.IsDeleted))
            .Include(e => e.Tickets.Where(t => !t.IsDeleted))
            .AsSplitQuery();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower();
            query = query.Where(e =>
                e.EventName.ToLower().Contains(term) ||
                e.EventCode.ToLower().Contains(term) ||
                (e.Description != null && e.Description.ToLower().Contains(term)) ||
                (e.Tags != null && e.Tags.ToLower().Contains(term)));
        }

        if (sportId.HasValue)
            query = query.Where(e => e.SportId == sportId.Value);

        if (academyId.HasValue)
            query = query.Where(e => e.AcademyId == academyId.Value);

        if (coachId.HasValue)
            query = query.Where(e => e.Coaches.Any(c => c.CoachId == coachId.Value && !c.IsDeleted));

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(e => e.EventType != null && e.EventType.Name == eventType);

        if (!string.IsNullOrWhiteSpace(category))
            query = query.Where(e => e.EventCategory != null && e.EventCategory.Name == category);

        if (dateFrom.HasValue)
            query = query.Where(e => e.StartDate >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(e => e.EndDate <= dateTo.Value);

        if (minPrice.HasValue)
            query = query.Where(e => e.RegistrationFee >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(e => e.RegistrationFee <= maxPrice.Value);

        if (minRating.HasValue)
            query = query.Where(e => e.Feedbacks.Any(f => !f.IsDeleted));

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(e => e.Venues.Any(v => !v.IsDeleted && v.City != null && v.City.ToLower() == city.ToLower()));

        if (!string.IsNullOrWhiteSpace(state))
            query = query.Where(e => e.Venues.Any(v => !v.IsDeleted && v.State != null && v.State.ToLower() == state.ToLower()));

        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(e => e.Venues.Any(v => !v.IsDeleted && v.Country != null && v.Country.ToLower() == country.ToLower()));

        if (!string.IsNullOrWhiteSpace(registrationStatus))
        {
            query = registrationStatus.ToLower() switch
            {
                "open" => query.Where(e => e.Status == EventStatus.RegistrationOpen),
                "closed" => query.Where(e => e.Status == EventStatus.RegistrationClosed),
                "upcoming" => query.Where(e => e.RegistrationOpenDate > DateTime.UtcNow),
                _ => query
            };
        }

        return query;
    }

    private static IQueryable<Event> ApplySorting(IQueryable<Event> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLower() switch
        {
            "popularity" => sortDescending
                ? query.OrderByDescending(e => e.Registrations.Count(r => r.Status == EventRegistrationStatus.Approved))
                : query.OrderBy(e => e.Registrations.Count(r => r.Status == EventRegistrationStatus.Approved)),
            "recentlyadded" or "newest" => sortDescending
                ? query.OrderBy(e => e.CreatedAt)
                : query.OrderByDescending(e => e.CreatedAt),
            "highestrated" => sortDescending
                ? query.OrderBy(e => e.Feedbacks.Count(f => !f.IsDeleted))
                : query.OrderByDescending(e => e.Feedbacks.Count(f => !f.IsDeleted)),
            "alphabetical" or "name" => sortDescending
                ? query.OrderByDescending(e => e.EventName)
                : query.OrderBy(e => e.EventName),
            "registrationclosingsoon" => query.OrderBy(e => e.RegistrationCloseDate),
            "fee" or "price" => sortDescending
                ? query.OrderByDescending(e => e.RegistrationFee)
                : query.OrderBy(e => e.RegistrationFee),
            _ => sortDescending
                ? query.OrderByDescending(e => e.StartDate)
                : query.OrderBy(e => e.StartDate)
        };
    }

    private static double CalculateDistance(decimal lat1, decimal lng1, decimal lat2, decimal lng2)
    {
        const double R = 6371;
        var dLat = ToRadians((double)(lat2 - lat1));
        var dLng = ToRadians((double)(lng2 - lng1));
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians((double)lat1)) * Math.Cos(ToRadians((double)lat2)) *
                Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;
}
