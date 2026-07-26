using Microsoft.EntityFrameworkCore;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Infrastructure.Persistence.Repositories;

public class AcademySearchRepository : Repository<Academy>, IAcademySearchRepository
{
    public AcademySearchRepository(ApplicationDbContext context) : base(context) { }

    public async Task<(IReadOnlyList<Academy> Academies, int TotalCount)> SearchAcademiesAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = Context.Academies
            .AsNoTracking()
            .Include(a => a.Contact)
            .Include(a => a.OperatingHours)
            .Include(a => a.AcademySports).ThenInclude(as2 => as2.Sport).ThenInclude(s => s!.SportCategory)
            .Include(a => a.Facilities)
            .Include(a => a.Memberships)
            .Include(a => a.Verification)
            .Include(a => a.Branches)
            .AsSplitQuery()
            .Where(a => !a.IsDeleted);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLowerInvariant();
            query = query.Where(a =>
                EF.Functions.Like(a.Name.ToLower(), $"%{term}%") ||
                (a.LegalName != null && EF.Functions.Like(a.LegalName.ToLower(), $"%{term}%")) ||
                (a.Description != null && EF.Functions.Like(a.Description.ToLower(), $"%{term}%")) ||
                (a.Contact != null && a.Contact.City != null && EF.Functions.Like(a.Contact.City.ToLower(), $"%{term}%")) ||
                (a.Contact != null && a.Contact.State != null && EF.Functions.Like(a.Contact.State.ToLower(), $"%{term}%")) ||
                EF.Functions.Like(a.AcademyCode.ToLower(), $"%{term}%"));
        }

        if (!string.IsNullOrWhiteSpace(name))
        {
            var nameLower = name.ToLowerInvariant();
            query = query.Where(a => EF.Functions.Like(a.Name.ToLower(), $"%{nameLower}%") ||
                (a.LegalName != null && EF.Functions.Like(a.LegalName.ToLower(), $"%{nameLower}%")));
        }

        if (!string.IsNullOrWhiteSpace(registrationNumber))
        {
            var regLower = registrationNumber.ToLowerInvariant();
            query = query.Where(a => a.RegistrationNumber != null &&
                EF.Functions.Like(a.RegistrationNumber.ToLower(), $"%{regLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(academyCode))
        {
            var code = academyCode.ToLowerInvariant();
            query = query.Where(a => EF.Functions.Like(a.AcademyCode.ToLower(), $"%{code}%"));
        }

        if (!string.IsNullOrWhiteSpace(country))
        {
            var countryLower = country.ToLowerInvariant();
            query = query.Where(a => a.Contact != null && a.Contact.Country != null &&
                EF.Functions.Like(a.Contact.Country.ToLower(), $"%{countryLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(state))
        {
            var stateLower = state.ToLowerInvariant();
            query = query.Where(a => a.Contact != null && a.Contact.State != null &&
                EF.Functions.Like(a.Contact.State.ToLower(), $"%{stateLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(city))
        {
            var cityLower = city.ToLowerInvariant();
            query = query.Where(a => a.Contact != null && a.Contact.City != null &&
                EF.Functions.Like(a.Contact.City.ToLower(), $"%{cityLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(district))
        {
            var districtLower = district.ToLowerInvariant();
            query = query.Where(a => a.Contact != null && a.Contact.Address != null &&
                EF.Functions.Like(a.Contact.Address.ToLower(), $"%{districtLower}%"));
        }

        if (!string.IsNullOrWhiteSpace(pinCode))
        {
            var pinLower = pinCode.ToLowerInvariant();
            query = query.Where(a => a.Contact != null && a.Contact.PostalCode != null &&
                EF.Functions.Like(a.Contact.PostalCode.ToLower(), $"%{pinLower}%"));
        }

        if (latitude.HasValue && longitude.HasValue && radiusKm.HasValue)
        {
            var lat = latitude.Value;
            var lon = longitude.Value;
            var radius = radiusKm.Value;

            var latMin = lat - radius / 111m;
            var latMax = lat + radius / 111m;
            var cosLat = (decimal)Math.Cos((double)lat * Math.PI / 180.0);
            var lonMin = lon - radius / (111m * cosLat);
            var lonMax = lon + radius / (111m * cosLat);

            query = query.Where(a => a.Contact != null && a.Contact.Latitude != null && a.Contact.Longitude != null &&
                a.Contact.Latitude >= latMin && a.Contact.Latitude <= latMax &&
                a.Contact.Longitude >= lonMin && a.Contact.Longitude <= lonMax);
        }

        if (!string.IsNullOrWhiteSpace(sportName))
        {
            var sportLower = sportName.ToLowerInvariant();
            query = query.Where(a => a.AcademySports.Any(as2 =>
                as2.Sport != null && EF.Functions.Like(as2.Sport.Name.ToLower(), $"%{sportLower}%")));
        }

        if (!string.IsNullOrWhiteSpace(sportCategory))
        {
            var catLower = sportCategory.ToLowerInvariant();
            query = query.Where(a => a.AcademySports.Any(as2 =>
                as2.Sport != null && as2.Sport.SportCategory != null &&
                EF.Functions.Like(as2.Sport.SportCategory.Name.ToLower(), $"%{catLower}%")));
        }

        if (hasSwimmingPool.HasValue && hasSwimmingPool.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%pool%") ||
                EF.Functions.Like(f.FacilityName.ToLower(), "%swimming%")));
        }

        if (hasIndoorStadium.HasValue && hasIndoorStadium.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%indoor%") ||
                EF.Functions.Like(f.FacilityName.ToLower(), "%stadium%")));
        }

        if (hasCricketGround.HasValue && hasCricketGround.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%cricket%")));
        }

        if (hasFootballGround.HasValue && hasFootballGround.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%football%")));
        }

        if (hasGym.HasValue && hasGym.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                f.FacilityType == AcademyFacilityType.Gym ||
                EF.Functions.Like(f.FacilityName.ToLower(), "%gym%")));
        }

        if (hasYogaHall.HasValue && hasYogaHall.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%yoga%")));
        }

        if (hasParking.HasValue && hasParking.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%parking%")));
        }

        if (hasMedicalRoom.HasValue && hasMedicalRoom.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%medical%") ||
                EF.Functions.Like(f.FacilityName.ToLower(), "%first aid%")));
        }

        if (hasWifi.HasValue && hasWifi.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%wifi%")));
        }

        if (hasCafeteria.HasValue && hasCafeteria.Value)
        {
            query = query.Where(a => a.Facilities.Any(f =>
                EF.Functions.Like(f.FacilityName.ToLower(), "%cafeteria%") ||
                EF.Functions.Like(f.FacilityName.ToLower(), "%canteen%")));
        }

        if (verifiedOnly.HasValue && verifiedOnly.Value)
        {
            query = query.Where(a => a.VerificationStatus == VerificationStatus.Verified);
        }

        if (governmentRegisteredOnly.HasValue && governmentRegisteredOnly.Value)
        {
            query = query.Where(a => a.RegistrationNumber != null);
        }

        if (minEstablishmentYears.HasValue)
        {
            var cutoff = DateTime.UtcNow.AddYears(-minEstablishmentYears.Value);
            query = query.Where(a => a.EstablishedDate != null && a.EstablishedDate <= cutoff);
        }

        if (minMembershipPrice.HasValue)
        {
            query = query.Where(a => a.Memberships.Any(m => m.Price >= minMembershipPrice.Value));
        }

        if (maxMembershipPrice.HasValue)
        {
            query = query.Where(a => a.Memberships.Any(m => m.Price <= maxMembershipPrice.Value));
        }

        if (minCoaches.HasValue)
        {
            query = query.Where(a =>
                Context.CoachAcademies.Count(ca => ca.AcademyId == a.Id) >= minCoaches.Value);
        }

        if (minAthletes.HasValue)
        {
            query = query.Where(a =>
                Context.AthleteAcademies.Count(aa => aa.AcademyId == a.Id) >= minAthletes.Value);
        }

        if (minBranches.HasValue)
        {
            query = query.Where(a => a.Branches.Count >= minBranches.Value);
        }

        if (openNow.HasValue && openNow.Value)
        {
            var now = TimeOnly.FromDateTime(DateTime.UtcNow);
            var dayOfWeek = DateTime.UtcNow.DayOfWeek;

            query = query.Where(a => a.OperatingHours != null && (
                (dayOfWeek == DayOfWeek.Monday && a.OperatingHours.MondayOpening != null && a.OperatingHours.MondayClosing != null && now >= a.OperatingHours.MondayOpening && now <= a.OperatingHours.MondayClosing) ||
                (dayOfWeek == DayOfWeek.Tuesday && a.OperatingHours.TuesdayOpening != null && a.OperatingHours.TuesdayClosing != null && now >= a.OperatingHours.TuesdayOpening && now <= a.OperatingHours.TuesdayClosing) ||
                (dayOfWeek == DayOfWeek.Wednesday && a.OperatingHours.WednesdayOpening != null && a.OperatingHours.WednesdayClosing != null && now >= a.OperatingHours.WednesdayOpening && now <= a.OperatingHours.WednesdayClosing) ||
                (dayOfWeek == DayOfWeek.Thursday && a.OperatingHours.ThursdayOpening != null && a.OperatingHours.ThursdayClosing != null && now >= a.OperatingHours.ThursdayOpening && now <= a.OperatingHours.ThursdayClosing) ||
                (dayOfWeek == DayOfWeek.Friday && a.OperatingHours.FridayOpening != null && a.OperatingHours.FridayClosing != null && now >= a.OperatingHours.FridayOpening && now <= a.OperatingHours.FridayClosing) ||
                (dayOfWeek == DayOfWeek.Saturday && a.OperatingHours.SaturdayOpening != null && a.OperatingHours.SaturdayClosing != null && now >= a.OperatingHours.SaturdayOpening && now <= a.OperatingHours.SaturdayClosing) ||
                (dayOfWeek == DayOfWeek.Sunday && a.OperatingHours.SundayOpening != null && a.OperatingHours.SundayClosing != null && now >= a.OperatingHours.SundayOpening && now <= a.OperatingHours.SundayClosing)));
        }

        if (weekendOpen.HasValue && weekendOpen.Value)
        {
            query = query.Where(a => a.OperatingHours != null && (
                (a.OperatingHours.SaturdayOpening != null && a.OperatingHours.SaturdayClosing != null) ||
                (a.OperatingHours.SundayOpening != null && a.OperatingHours.SundayClosing != null)));
        }

        query = ApplySorting(query, sortBy, latitude, longitude);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items.AsReadOnly(), totalCount);
    }

    public async Task<IReadOnlyList<Academy>> GetNearbyAcademiesAsync(
        decimal latitude,
        decimal longitude,
        decimal radiusKm,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var latMin = latitude - radiusKm / 111m;
        var latMax = latitude + radiusKm / 111m;
        var cosLat = (decimal)Math.Cos((double)latitude * Math.PI / 180.0);
        var lonMin = longitude - radiusKm / (111m * cosLat);
        var lonMax = longitude + radiusKm / (111m * cosLat);

        return await Context.Academies
            .AsNoTracking()
            .Include(a => a.Contact)
            .Include(a => a.AcademySports).ThenInclude(as2 => as2.Sport)
            .Include(a => a.Facilities)
            .AsSplitQuery()
            .Where(a => !a.IsDeleted && a.Contact != null &&
                a.Contact.Latitude != null && a.Contact.Longitude != null &&
                a.Contact.Latitude >= latMin && a.Contact.Latitude <= latMax &&
                a.Contact.Longitude >= lonMin && a.Contact.Longitude <= lonMax)
            .OrderBy(a =>
                (2.0 * 6371.0 * Math.Asin(Math.Sqrt(
                    Math.Pow(Math.Sin(((double)a.Contact!.Latitude!.Value - (double)latitude) * Math.PI / 360.0), 2) +
                    Math.Cos((double)latitude * Math.PI / 180.0) * Math.Cos((double)a.Contact.Latitude.Value * Math.PI / 180.0) *
                    Math.Pow(Math.Sin(((double)a.Contact.Longitude!.Value - (double)longitude) * Math.PI / 360.0), 2)))))
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Academy>> GetAutocompleteSuggestionsAsync(
        string prefix,
        int limit,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prefix) || prefix.Length < 2)
            return [];

        var term = prefix.ToLowerInvariant();

        return await Context.Academies
            .AsNoTracking()
            .Include(a => a.Contact)
            .Where(a => !a.IsDeleted && (
                EF.Functions.Like(a.Name.ToLower(), $"%{term}%") ||
                EF.Functions.Like(a.AcademyCode.ToLower(), $"%{term}%") ||
                (a.Contact != null && a.Contact.City != null && EF.Functions.Like(a.Contact.City.ToLower(), $"%{term}%"))))
            .OrderBy(a => a.Name)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Academy>> GetSimilarAcademiesAsync(
        Guid academyId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        var academy = await Context.Academies
            .AsNoTracking()
            .Include(a => a.AcademySports).ThenInclude(as2 => as2.Sport)
            .Include(a => a.Contact)
            .FirstOrDefaultAsync(a => a.Id == academyId, cancellationToken);

        if (academy is null)
            return [];

        var sportIds = academy.AcademySports.Select(as2 => as2.SportId).ToList();

        var query = Context.Academies
            .AsNoTracking()
            .Include(a => a.Contact)
            .Include(a => a.AcademySports).ThenInclude(as2 => as2.Sport)
            .Include(a => a.Facilities)
            .Include(a => a.Memberships)
            .AsSplitQuery()
            .Where(a => a.Id != academyId && !a.IsDeleted && a.VerificationStatus == VerificationStatus.Verified);

        if (sportIds.Count > 0)
        {
            query = query.Where(a => a.AcademySports.Any(as2 => sportIds.Contains(as2.SportId)));
        }

        query = query.OrderByDescending(a =>
            a.AcademySports.Count(as2 => sportIds.Contains(as2.SportId)))
            .ThenByDescending(a => a.VerificationStatus == VerificationStatus.Verified)
            .Take(limit);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Academy>> GetPopularAcademiesAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await Context.Academies
            .AsNoTracking()
            .Include(a => a.Contact)
            .Include(a => a.AcademySports).ThenInclude(as2 => as2.Sport)
            .Include(a => a.Facilities)
            .AsSplitQuery()
            .Where(a => !a.IsDeleted && a.VerificationStatus == VerificationStatus.Verified)
            .OrderByDescending(a => a.AcademySports.Count)
            .ThenByDescending(a => a.Facilities.Count)
            .ThenByDescending(a => a.CreatedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Academy>> GetFrequentlyViewedAcademiesAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await Context.AcademyViews
            .AsNoTracking()
            .Where(v => !v.IsDeleted)
            .GroupBy(v => v.AcademyId)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key)
            .Take(limit)
            .Join(Context.Academies.AsNoTracking().Where(a => !a.IsDeleted),
                viewAcademyId => viewAcademyId,
                academy => academy.Id,
                (viewAcademyId, academy) => academy)
            .Include(a => a.Contact)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<string>> GetPopularSearchTermsAsync(
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await Context.RecentAcademySearches
            .AsNoTracking()
            .Where(s => !s.IsDeleted && !string.IsNullOrWhiteSpace(s.SearchTerm))
            .GroupBy(s => s.SearchTerm)
            .OrderByDescending(g => g.Count())
            .Select(g => g.Key!)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task SaveSearchAsync(SavedAcademySearch search, CancellationToken cancellationToken = default)
    {
        var existing = await Context.SavedAcademySearches
            .FirstOrDefaultAsync(s => s.UserId == search.UserId && s.SearchName == search.SearchName && !s.IsDeleted, cancellationToken);

        if (existing is not null)
        {
            existing.SearchTerm = search.SearchTerm;
            existing.Name = search.Name;
            existing.City = search.City;
            existing.State = search.State;
            existing.Country = search.Country;
            existing.District = search.District;
            existing.PinCode = search.PinCode;
            existing.SportName = search.SportName;
            existing.SportCategory = search.SportCategory;
            existing.FacilityType = search.FacilityType;
            existing.HasSwimmingPool = search.HasSwimmingPool;
            existing.HasIndoorStadium = search.HasIndoorStadium;
            existing.HasCricketGround = search.HasCricketGround;
            existing.HasFootballGround = search.HasFootballGround;
            existing.HasGym = search.HasGym;
            existing.HasYogaHall = search.HasYogaHall;
            existing.HasParking = search.HasParking;
            existing.HasMedicalRoom = search.HasMedicalRoom;
            existing.HasWifi = search.HasWifi;
            existing.HasCafeteria = search.HasCafeteria;
            existing.VerifiedOnly = search.VerifiedOnly;
            existing.GovernmentRegisteredOnly = search.GovernmentRegisteredOnly;
            existing.OpenNow = search.OpenNow;
            existing.WeekendOpen = search.WeekendOpen;
            existing.MinMembershipPrice = search.MinMembershipPrice;
            existing.MaxMembershipPrice = search.MaxMembershipPrice;
            existing.MinRating = search.MinRating;
            existing.ResultCount = search.ResultCount;
            Context.SavedAcademySearches.Update(existing);
        }
        else
        {
            await Context.SavedAcademySearches.AddAsync(search, cancellationToken);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SavedAcademySearch>> GetSavedSearchesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await Context.SavedAcademySearches
            .AsNoTracking()
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteSavedSearchAsync(
        Guid searchId,
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var search = await Context.SavedAcademySearches
            .FirstOrDefaultAsync(s => s.Id == searchId && s.UserId == userId && !s.IsDeleted, cancellationToken);

        if (search is null)
            return;

        search.IsDeleted = true;
        Context.SavedAcademySearches.Update(search);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<SavedAcademySearch?> GetSavedSearchByIdAsync(
        Guid searchId,
        CancellationToken cancellationToken = default)
    {
        return await Context.SavedAcademySearches
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == searchId && !s.IsDeleted, cancellationToken);
    }

    public async Task RecordSearchAsync(
        RecentAcademySearch search,
        CancellationToken cancellationToken = default)
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
        var oldSearches = await Context.RecentAcademySearches
            .Where(s => s.UserId == search.UserId && s.SearchedAt < thirtyDaysAgo && !s.IsDeleted)
            .ToListAsync(cancellationToken);

        if (oldSearches.Count > 0)
        {
            Context.RecentAcademySearches.RemoveRange(oldSearches);
        }

        var recentCount = await Context.RecentAcademySearches
            .CountAsync(s => s.UserId == search.UserId && !s.IsDeleted, cancellationToken);

        if (recentCount >= 50)
        {
            var oldest = await Context.RecentAcademySearches
                .Where(s => s.UserId == search.UserId && !s.IsDeleted)
                .OrderBy(s => s.SearchedAt)
                .Take(recentCount - 49)
                .ToListAsync(cancellationToken);

            Context.RecentAcademySearches.RemoveRange(oldest);
        }

        await Context.RecentAcademySearches.AddAsync(search, cancellationToken);
        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<RecentAcademySearch>> GetRecentSearchesAsync(
        Guid userId,
        int limit,
        CancellationToken cancellationToken = default)
    {
        return await Context.RecentAcademySearches
            .AsNoTracking()
            .Where(s => s.UserId == userId && !s.IsDeleted)
            .OrderByDescending(s => s.SearchedAt)
            .Take(limit)
            .ToListAsync(cancellationToken);
    }

    public async Task TrackViewAsync(
        AcademyView view,
        CancellationToken cancellationToken = default)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var existingView = await Context.AcademyViews
            .FirstOrDefaultAsync(v =>
                v.AcademyId == view.AcademyId &&
                v.ViewedByUserId == view.ViewedByUserId &&
                DateOnly.FromDateTime(v.ViewedAt) == today &&
                !v.IsDeleted,
                cancellationToken);

        if (existingView is not null)
        {
            existingView.ViewedAt = view.ViewedAt;
            existingView.Source = view.Source;
            Context.AcademyViews.Update(existingView);
        }
        else
        {
            await Context.AcademyViews.AddAsync(view, cancellationToken);
        }

        await Context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> GetViewCountAsync(
        Guid academyId,
        CancellationToken cancellationToken = default)
    {
        return await Context.AcademyViews
            .AsNoTracking()
            .CountAsync(v => v.AcademyId == academyId && !v.IsDeleted, cancellationToken);
    }

    private static IQueryable<Academy> ApplySorting(
        IQueryable<Academy> query,
        string? sortBy,
        decimal? latitude,
        decimal? longitude)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "nearest" when latitude.HasValue && longitude.HasValue =>
                query.OrderBy(a =>
                    (2.0 * 6371.0 * Math.Asin(Math.Sqrt(
                        Math.Pow(Math.Sin(((double)a.Contact!.Latitude!.Value - (double)latitude.Value) * Math.PI / 360.0), 2) +
                        Math.Cos((double)latitude.Value * Math.PI / 180.0) * Math.Cos((double)a.Contact.Latitude.Value * Math.PI / 180.0) *
                        Math.Pow(Math.Sin(((double)a.Contact.Longitude!.Value - (double)longitude.Value) * Math.PI / 360.0), 2))))),
            "highestrated" or "rating" =>
                query.OrderByDescending(a => a.Memberships.Count)
                    .ThenByDescending(a => a.VerificationStatus == VerificationStatus.Verified),
            "newest" or "createdat" =>
                query.OrderByDescending(a => a.CreatedAt),
            "alphabetical" or "name" =>
                query.OrderBy(a => a.Name),
            "popular" =>
                query.OrderByDescending(a => a.AcademySports.Count)
                    .ThenByDescending(a => a.Facilities.Count),
            "oldest" =>
                query.OrderBy(a => a.CreatedAt),
            _ =>
                query.OrderByDescending(a => a.CreatedAt)
        };
    }
}
