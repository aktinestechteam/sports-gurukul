using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.AdvancedSearchAcademies;

public class AdvancedSearchAcademiesQueryHandler : IRequestHandler<AdvancedSearchAcademiesQuery, Result<AcademySearchPageResultDto>>
{
    private readonly IAcademySearchRepository _academySearchRepository;
    private readonly ILogger<AdvancedSearchAcademiesQueryHandler> _logger;

    public AdvancedSearchAcademiesQueryHandler(
        IAcademySearchRepository academySearchRepository,
        ILogger<AdvancedSearchAcademiesQueryHandler> logger)
    {
        _academySearchRepository = academySearchRepository;
        _logger = logger;
    }

    public async Task<Result<AcademySearchPageResultDto>> Handle(AdvancedSearchAcademiesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Advanced academy search requested: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var (academies, totalCount) = await _academySearchRepository.SearchAcademiesAsync(
            request.SearchTerm,
            request.Name,
            request.RegistrationNumber,
            request.AcademyCode,
            request.Country,
            request.State,
            request.City,
            request.District,
            request.PinCode,
            request.Latitude,
            request.Longitude,
            request.RadiusKm,
            request.SportName,
            request.SportCategory,
            request.HasSwimmingPool,
            request.HasIndoorStadium,
            request.HasCricketGround,
            request.HasFootballGround,
            request.HasGym,
            request.HasYogaHall,
            request.HasParking,
            request.HasMedicalRoom,
            request.HasWifi,
            request.HasCafeteria,
            request.VerifiedOnly,
            request.GovernmentRegisteredOnly,
            request.MinEstablishmentYears,
            request.MinMembershipPrice,
            request.MaxMembershipPrice,
            request.MinRating,
            request.MinCoaches,
            request.MinAthletes,
            request.MinBranches,
            request.OpenNow,
            request.WeekendOpen,
            request.SortBy,
            request.Page,
            request.PageSize,
            cancellationToken);

        var items = academies.Select(a => MapToSearchResultDto(a, request.Latitude, request.Longitude)).ToList();

        var response = new AcademySearchPageResultDto
        {
            Items = items,
            TotalRecords = totalCount,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize),
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Result<AcademySearchPageResultDto>.Success(response);
    }

    private static AcademySearchResultDto MapToSearchResultDto(Academy academy, decimal? userLatitude, decimal? userLongitude)
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

        double? distanceKm = null;
        var contactLat = academy.Contact?.Latitude;
        var contactLng = academy.Contact?.Longitude;
        if (userLatitude.HasValue && userLongitude.HasValue && contactLat.HasValue && contactLng.HasValue)
        {
            distanceKm = CalculateHaversineDistance(
                (double)userLatitude.Value, (double)userLongitude.Value,
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
