using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface IAcademySearchRepository
{
    Task<(IReadOnlyList<Academy> Academies, int TotalCount)> SearchAcademiesAsync(
        string? searchTerm,
        string? name,
        string? registrationNumber,
        string? academyCode,
        string? country,
        string? state,
        string? city,
        string? district,
        string? pinCode,
        decimal? latitude,
        decimal? longitude,
        decimal? radiusKm,
        string? sportName,
        string? sportCategory,
        bool? hasSwimmingPool,
        bool? hasIndoorStadium,
        bool? hasCricketGround,
        bool? hasFootballGround,
        bool? hasGym,
        bool? hasYogaHall,
        bool? hasParking,
        bool? hasMedicalRoom,
        bool? hasWifi,
        bool? hasCafeteria,
        bool? verifiedOnly,
        bool? governmentRegisteredOnly,
        int? minEstablishmentYears,
        decimal? minMembershipPrice,
        decimal? maxMembershipPrice,
        decimal? minRating,
        int? minCoaches,
        int? minAthletes,
        int? minBranches,
        bool? openNow,
        bool? weekendOpen,
        string? sortBy,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Academy>> GetNearbyAcademiesAsync(
        decimal latitude,
        decimal longitude,
        decimal radiusKm,
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Academy>> GetAutocompleteSuggestionsAsync(
        string prefix,
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Academy>> GetSimilarAcademiesAsync(
        Guid academyId,
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Academy>> GetPopularAcademiesAsync(
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Academy>> GetFrequentlyViewedAcademiesAsync(
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetPopularSearchTermsAsync(
        int limit,
        CancellationToken cancellationToken = default);

    Task SaveSearchAsync(SavedAcademySearch search, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<SavedAcademySearch>> GetSavedSearchesAsync(Guid userId, CancellationToken cancellationToken = default);
    Task DeleteSavedSearchAsync(Guid searchId, Guid userId, CancellationToken cancellationToken = default);
    Task<SavedAcademySearch?> GetSavedSearchByIdAsync(Guid searchId, CancellationToken cancellationToken = default);

    Task RecordSearchAsync(RecentAcademySearch search, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RecentAcademySearch>> GetRecentSearchesAsync(Guid userId, int limit, CancellationToken cancellationToken = default);

    Task TrackViewAsync(AcademyView view, CancellationToken cancellationToken = default);
    Task<int> GetViewCountAsync(Guid academyId, CancellationToken cancellationToken = default);
}
