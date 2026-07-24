using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class CoachSearchRepository : Repository<Coach>, ICoachSearchRepository
{
    public CoachSearchRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(IReadOnlyList<Coach> Items, int TotalCount)> SearchCoachesAsync(
        string? searchTerm,
        string? name,
        string? coachCode,
        string? email,
        string? mobile,
        string? sportName,
        Guid[]? sportIds,
        string? sportCategory,
        CoachingLevel? coachingLevel,
        int? minExperience,
        int? maxExperience,
        string? certificationName,
        VerificationStatus? certificationStatus,
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
        CancellationToken cancellationToken = default)
    {
        var query = Context.Coaches
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.CoachSports).ThenInclude(cs => cs.Sport).ThenInclude(s => s!.SportCategory)
            .Include(c => c.Certifications)
            .Include(c => c.Availability)
            .Include(c => c.Location)
            .AsSplitQuery()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(c =>
                (c.User.FullName != null && EF.Functions.Like(c.User.FullName.ToLower(), $"%{term}%")) ||
                (c.User.Email != null && EF.Functions.Like(c.User.Email.ToLower(), $"%{term}%")) ||
                (c.User.PhoneNumber != null && EF.Functions.Like(c.User.PhoneNumber, $"%{term}%")) ||
                (c.CoachCode != null && EF.Functions.Like(c.CoachCode.ToLower(), $"%{term}%")));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var nameLower = name.ToLowerInvariant();
            query = query.Where(c => c.User.FullName != null && EF.Functions.Like(c.User.FullName.ToLower(), $"%{nameLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(coachCode))
        {
            var code = coachCode.ToLowerInvariant();
            query = query.Where(c => c.CoachCode != null && EF.Functions.Like(c.CoachCode.ToLower(), $"%{code}%"));
        }

        if (!string.IsNullOrWhiteSpace(email))
        {
            var emailLower = email.ToLowerInvariant();
            query = query.Where(c => c.User.Email != null && EF.Functions.Like(c.User.Email.ToLower(), $"%{emailLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(mobile))
        {
            query = query.Where(c => c.User.PhoneNumber != null && c.User.PhoneNumber.Contains(mobile));
        }

        if (!string.IsNullOrWhiteSpace(sportName))
        {
            var sportNameLower = sportName.ToLowerInvariant();
            query = query.Where(c => c.CoachSports.Any(cs =>
                cs.Sport != null && EF.Functions.Like(cs.Sport.Name.ToLower(), $"%{sportNameLower}%")));
        }

        if (sportIds is { Length: > 0 })
        {
            query = query.Where(c => c.CoachSports.Any(cs => sportIds.Contains(cs.SportId)));
        }

        if (!string.IsNullOrWhiteSpace(sportCategory))
        {
            var catLower = sportCategory.ToLowerInvariant();
            query = query.Where(c => c.CoachSports.Any(cs =>
                cs.Sport != null && cs.Sport.SportCategory != null &&
                EF.Functions.Like(cs.Sport.SportCategory.Name.ToLower(), $"%{catLower}%")));
        }

        if (coachingLevel.HasValue)
        {
            query = query.Where(c => c.CoachingLevel == coachingLevel.Value);
        }

        if (minExperience.HasValue)
        {
            query = query.Where(c => c.YearsOfExperience >= minExperience.Value);
        }

        if (maxExperience.HasValue)
        {
            query = query.Where(c => c.YearsOfExperience <= maxExperience.Value);
        }

        if (!string.IsNullOrWhiteSpace(certificationName))
        {
            var certLower = certificationName.ToLowerInvariant();
            query = query.Where(c => c.Certifications.Any(cert =>
                EF.Functions.Like(cert.CertificationName.ToLower(), $"%{certLower}%")));
        }

        if (certificationStatus.HasValue)
        {
            query = query.Where(c => c.Certifications.Any(cert =>
                cert.VerificationStatus == certificationStatus.Value));
        }

        if (!string.IsNullOrWhiteSpace(currentOrganization))
        {
            var orgLower = currentOrganization.ToLowerInvariant();
            query = query.Where(c => c.CurrentOrganization != null &&
                EF.Functions.Like(c.CurrentOrganization.ToLower(), $"%{orgLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(highestQualification))
        {
            var qualLower = highestQualification.ToLowerInvariant();
            query = query.Where(c => c.HighestQualification != null &&
                EF.Functions.Like(c.HighestQualification.ToLower(), $"%{qualLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            var countryLower = country.ToLowerInvariant();
            query = query.Where(c => c.Location != null && c.Location.Country != null &&
                EF.Functions.Like(c.Location.Country.ToLower(), $"%{countryLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(state))
        {
            var stateLower = state.ToLowerInvariant();
            query = query.Where(c => c.Location != null && c.Location.State != null &&
                EF.Functions.Like(c.Location.State.ToLower(), $"%{stateLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var cityLower = city.ToLowerInvariant();
            query = query.Where(c => c.Location != null && c.Location.City != null &&
                EF.Functions.Like(c.Location.City.ToLower(), $"%{cityLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(district))
        {
            var districtLower = district.ToLowerInvariant();
            query = query.Where(c => c.Location != null && c.Location.District != null &&
                EF.Functions.Like(c.Location.District.ToLower(), $"%{districtLower}%"));
        }

        if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
        {
            var lat = latitude.Value;
            var lon = longitude.Value;
            var radius = (decimal)radiusKm.Value;

            query = query.Where(c => c.Location != null && c.Location.Latitude != null && c.Location.Longitude != null);

            var latMin = lat - radius / 111m;
            var latMax = lat + radius / 111m;
            var cosLat = (decimal)Math.Cos((double)lat * Math.PI / 180.0);
            var lonMin = lon - radius / (111m * cosLat);
            var lonMax = lon + radius / (111m * cosLat);

            query = query.Where(c => c.Location!.Latitude >= latMin && c.Location.Latitude <= latMax &&
                c.Location.Longitude >= lonMin && c.Location.Longitude <= lonMax);
        }

        if (availableToday.HasValue && availableToday.Value)
        {
            var today = DateTime.UtcNow.DayOfWeek.ToString();
            query = query.Where(c => c.Availability != null &&
                c.Availability.WeeklySchedule.Contains(today));
        }

        if (onlineAvailable.HasValue)
        {
            query = query.Where(c => c.Availability != null && c.Availability.OnlineAvailable == onlineAvailable.Value);
        }

        if (offlineAvailable.HasValue)
        {
            query = query.Where(c => c.Availability != null && c.Availability.OfflineAvailable == offlineAvailable.Value);
        }

        if (isVerified.HasValue)
        {
            query = query.Where(c => isVerified.Value
                ? c.VerificationStatus == VerificationStatus.Verified
                : c.VerificationStatus != VerificationStatus.Verified);
        }

        if (backgroundVerified.HasValue && backgroundVerified.Value)
        {
            query = query.Where(c => c.VerificationStatus == VerificationStatus.Verified &&
                c.Status == CoachStatus.Active);
        }

        if (!string.IsNullOrWhiteSpace(language))
        {
            var langLower = language.ToLowerInvariant();
            query = query.Where(c => c.PreferredLanguage != null &&
                EF.Functions.Like(c.PreferredLanguage.ToLower(), $"%{langLower}%"));
        }

        if (useCursorPagination && !string.IsNullOrWhiteSpace(cursor))
        {
            var cursorDate = JsonSerializer.Deserialize<DateTime>(cursor);
            query = query.Where(c => c.CreatedAt < cursorDate);
        }

        query = ApplySorting(query, sortBy, sortDescending);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = useCursorPagination
            ? await query.Take(pageSize + 1).ToListAsync(cancellationToken)
            : await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

        return (items.AsReadOnly(), totalCount);
    }

    public async Task<IReadOnlyList<Coach>> GetSimilarCoachesAsync(
        Guid coachId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var coach = await Context.Coaches
            .AsNoTracking()
            .Include(c => c.CoachSports).ThenInclude(cs => cs.Sport)
            .Include(c => c.Location)
            .FirstOrDefaultAsync(c => c.Id == coachId, cancellationToken);

        if (coach is null)
            return [];

        var sportIds = coach.CoachSports.Select(cs => cs.SportId).ToList();
        var coachingLevel = coach.CoachingLevel;

        var query = Context.Coaches
            .AsNoTracking()
            .Include(c => c.User)
            .Include(c => c.CoachSports).ThenInclude(cs => cs.Sport)
            .Include(c => c.Location)
            .AsSplitQuery()
            .Where(c => c.Id != coachId && !c.IsDeleted && c.Status == CoachStatus.Active);

        if (sportIds.Count > 0)
        {
            query = query.Where(c => c.CoachSports.Any(cs => sportIds.Contains(cs.SportId)));
        }

        query = query.OrderByDescending(c =>
            c.CoachSports.Count(cs => sportIds.Contains(cs.SportId)))
            .ThenByDescending(c => c.VerificationStatus == VerificationStatus.Verified)
            .ThenByDescending(c => c.YearsOfExperience)
            .Take(limit);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetSearchSuggestionsAsync(
        string prefix,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prefix) || prefix.Length < 2)
            return [];

        var term = prefix.ToLowerInvariant();

        var suggestions = await Context.Coaches
            .AsNoTracking()
            .Include(c => c.User)
            .Where(c => !c.IsDeleted && c.Status == CoachStatus.Active && (
                (c.User.FullName != null && EF.Functions.Like(c.User.FullName.ToLower(), $"%{term}%")) ||
                (c.CoachCode != null && EF.Functions.Like(c.CoachCode.ToLower(), $"%{term}%"))))
            .Select(c => new
            {
                c.User.FullName,
                c.CoachCode
            })
            .Take(limit)
            .ToListAsync(cancellationToken);

        var results = new List<string>();
        foreach (var s in suggestions)
        {
            if (!string.IsNullOrWhiteSpace(s.FullName) && !results.Contains(s.FullName))
                results.Add(s.FullName);
            if (!string.IsNullOrWhiteSpace(s.CoachCode) && !results.Contains(s.CoachCode))
                results.Add(s.CoachCode);
        }

        return results.Take(limit).ToList();
    }

    private static IQueryable<Coach> ApplySorting(IQueryable<Coach> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending
                ? query.OrderByDescending(c => c.User.FullName)
                : query.OrderBy(c => c.User.FullName),
            "experience" => sortDescending
                ? query.OrderByDescending(c => c.YearsOfExperience)
                : query.OrderBy(c => c.YearsOfExperience),
            "coachcode" => sortDescending
                ? query.OrderByDescending(c => c.CoachCode)
                : query.OrderBy(c => c.CoachCode),
            "status" => sortDescending
                ? query.OrderByDescending(c => c.Status)
                : query.OrderBy(c => c.Status),
            "createdat" or "recentlyjoined" => sortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt),
            "availability" => sortDescending
                ? query.OrderByDescending(c => c.Availability != null && c.Availability.OnlineAvailable)
                : query.OrderBy(c => c.Availability != null && c.Availability.OnlineAvailable),
            "rating" or "recommendation" => sortDescending
                ? query.OrderByDescending(c => c.VerificationStatus == VerificationStatus.Verified)
                    .ThenByDescending(c => c.YearsOfExperience)
                : query.OrderBy(c => c.VerificationStatus == VerificationStatus.Verified)
                    .ThenBy(c => c.YearsOfExperience),
            "alphabetical" => sortDescending
                ? query.OrderByDescending(c => c.User.FullName)
                : query.OrderBy(c => c.User.FullName),
            _ => sortDescending
                ? query.OrderByDescending(c => c.CreatedAt)
                : query.OrderBy(c => c.CreatedAt)
        };
    }
}
