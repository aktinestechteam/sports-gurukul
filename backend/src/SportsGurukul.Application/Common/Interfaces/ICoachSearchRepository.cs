using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Common.Interfaces;

public interface ICoachSearchRepository : IRepository<Coach>
{
    Task<(IReadOnlyList<Coach> Items, int TotalCount)> SearchCoachesAsync(
        string? searchTerm,
        string? name,
        string? coachCode,
        string? email,
        string? mobile,
        string? sportName,
        Guid[]? sportIds,
        string? sportCategory,
        Domain.Enums.CoachingLevel? coachingLevel,
        int? minExperience,
        int? maxExperience,
        string? certificationName,
        Domain.Enums.VerificationStatus? certificationStatus,
        string? currentOrganization,
        string? highestQualification,
        string? country,
        string? state,
        string? city,
        string? district,
        decimal? latitude,
        decimal? longitude,
        double? radiusKm,
        bool? availableToday,
        bool? onlineAvailable,
        bool? offlineAvailable,
        bool? isVerified,
        bool? backgroundVerified,
        string? language,
        string? sortBy,
        bool sortDescending,
        int page,
        int pageSize,
        string? cursor,
        bool useCursorPagination,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Coach>> GetSimilarCoachesAsync(
        Guid coachId,
        int limit,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<string>> GetSearchSuggestionsAsync(
        string prefix,
        int limit,
        CancellationToken cancellationToken = default);
}
