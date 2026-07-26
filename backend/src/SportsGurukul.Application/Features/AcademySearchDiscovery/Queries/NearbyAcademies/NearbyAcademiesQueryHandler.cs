using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.NearbyAcademies;

public class NearbyAcademiesQueryHandler : IRequestHandler<NearbyAcademiesQuery, Result<IReadOnlyList<AcademySearchResultDto>>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<NearbyAcademiesQueryHandler> _logger;

    public NearbyAcademiesQueryHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<NearbyAcademiesQueryHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AcademySearchResultDto>>> Handle(NearbyAcademiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Nearby academies search: Lat={Latitude}, Lng={Longitude}, Radius={RadiusKm}km", request.Latitude, request.Longitude, request.RadiusKm);

        var academies = await _academySearchRepository.GetNearbyAcademiesAsync(
            request.Latitude, request.Longitude, request.RadiusKm, request.Limit, cancellationToken);

        IEnumerable<Academy> filtered = academies;
        if (!string.IsNullOrWhiteSpace(request.SportName))
        {
            filtered = academies.Where(a =>
                a.AcademySports.Any(s =>
                    s.Sport.Name.Contains(request.SportName!, StringComparison.OrdinalIgnoreCase)));
        }

        var results = filtered.Select(a => MapToSearchResultDto(a, request.Latitude, request.Longitude)).ToList();

        return Result<IReadOnlyList<AcademySearchResultDto>>.Success(results);
    }

    private static AcademySearchResultDto MapToSearchResultDto(Academy academy, decimal userLatitude, decimal userLongitude)
    {
        var now = DateTime.UtcNow;
        var today = now.DayOfWeek;
        var currentTime = TimeOnly.FromDateTime(now);

        var operatingHours = academy.OperatingHours;
        bool isOpenNow = false;
        bool isWeekendOpen = false;

        if (operatingHours is not null)
        {
            isOpenNow = today switch
            {
                DayOfWeek.Monday => IsWithinOperatingHours(operatingHours.MondayOpening, operatingHours.MondayClosing, currentTime),
                DayOfWeek.Tuesday => IsWithinOperatingHours(operatingHours.TuesdayOpening, operatingHours.TuesdayClosing, currentTime),
                DayOfWeek.Wednesday => IsWithinOperatingHours(operatingHours.WednesdayOpening, operatingHours.WednesdayClosing, currentTime),
                DayOfWeek.Thursday => IsWithinOperatingHours(operatingHours.ThursdayOpening, operatingHours.ThursdayClosing, currentTime),
                DayOfWeek.Friday => IsWithinOperatingHours(operatingHours.FridayOpening, operatingHours.FridayClosing, currentTime),
                DayOfWeek.Saturday => IsWithinOperatingHours(operatingHours.SaturdayOpening, operatingHours.SaturdayClosing, currentTime),
                DayOfWeek.Sunday => IsWithinOperatingHours(operatingHours.SundayOpening, operatingHours.SundayClosing, currentTime),
                _ => false
            };

            isWeekendOpen = operatingHours.SaturdayOpening.HasValue || operatingHours.SundayOpening.HasValue;
        }

        var contactLat = academy.Contact?.Latitude;
        var contactLng = academy.Contact?.Longitude;
        double? distanceKm = null;
        if (contactLat.HasValue && contactLng.HasValue)
        {
            distanceKm = CalculateHaversineDistance(
                (double)userLatitude, (double)userLongitude,
                (double)contactLat.Value, (double)contactLng.Value);
        }

        int yearsEstablished = academy.EstablishedDate.HasValue
            ? (int)(now - academy.EstablishedDate.Value).TotalDays / 365
            : 0;

        var sports = academy.AcademySports?.Select(s => s.Sport?.Name ?? string.Empty)
            .Where(n => !string.IsNullOrEmpty(n)).ToList() ?? [];

        var facilityTypes = academy.Facilities?.Select(f => f.FacilityType.ToString())
            .Distinct().ToList() ?? [];

        var facilityNames = academy.Facilities?.Select(f => f.FacilityName)
            .Where(n => !string.IsNullOrEmpty(n)).Distinct().ToList() ?? [];

        var memberships = academy.Memberships?.ToList() ?? [];
        decimal? minMembershipPrice = memberships.Count > 0 ? memberships.Min(m => (decimal?)m.Price) : null;
        decimal? maxMembershipPrice = memberships.Count > 0 ? memberships.Max(m => (decimal?)m.Price) : null;

        return new AcademySearchResultDto
        {
            Id = academy.Id,
            AcademyCode = academy.AcademyCode,
            Name = academy.Name,
            Description = academy.Description,
            LogoUrl = academy.LogoUrl,
            BannerUrl = academy.BannerUrl,
            Email = academy.Email,
            Phone = academy.Phone,
            Status = academy.Status.ToString(),
            VerificationStatus = academy.VerificationStatus.ToString(),
            IsVerified = academy.VerificationStatus == Domain.Enums.VerificationStatus.Verified,
            IsGovernmentRegistered = !string.IsNullOrEmpty(academy.RegistrationNumber),
            EstablishedDate = academy.EstablishedDate,
            YearsEstablished = yearsEstablished,
            Country = academy.Contact?.Country,
            State = academy.Contact?.State,
            City = academy.Contact?.City,
            PinCode = academy.Contact?.PostalCode,
            Latitude = contactLat,
            Longitude = contactLng,
            DistanceKm = distanceKm,
            Sports = sports,
            FacilityTypes = facilityTypes,
            FacilityNames = facilityNames,
            MinMembershipPrice = minMembershipPrice,
            MaxMembershipPrice = maxMembershipPrice,
            TotalCoaches = 0,
            TotalAthletes = 0,
            TotalBranches = academy.Branches?.Count ?? 0,
            TotalFacilities = academy.Facilities?.Count ?? 0,
            TotalMemberships = memberships.Count,
            TotalReviews = 0,
            AverageRating = 0,
            ViewCount = 0,
            IsOpenNow = isOpenNow,
            IsWeekendOpen = isWeekendOpen,
            CreatedAt = academy.CreatedAt
        };
    }

    private static bool IsWithinOperatingHours(TimeOnly? opening, TimeOnly? closing, TimeOnly currentTime)
    {
        if (!opening.HasValue || !closing.HasValue)
            return false;

        return currentTime >= opening.Value && currentTime <= closing.Value;
    }

    private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371.0;
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);
        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180.0;
}
