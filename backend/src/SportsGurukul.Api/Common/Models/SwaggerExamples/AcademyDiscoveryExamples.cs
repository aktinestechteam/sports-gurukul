using SportsGurukul.Api.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

#region Academy Search Request Examples

/// <summary>
/// Swagger request example for <see cref="AdvancedAcademySearchRequest"/>.
/// </summary>
public class AdvancedAcademySearchRequestExample : IExamplesProvider<AdvancedAcademySearchRequest>
{
    public AdvancedAcademySearchRequest GetExamples() => new()
    {
        SearchTerm = "cricket",
        Name = "Elite Sports Academy",
        City = "Mumbai",
        State = "Maharashtra",
        Country = "India",
        SportName = "Cricket",
        SportCategory = "Team Sports",
        HasCricketGround = true,
        HasGym = true,
        HasParking = true,
        HasWifi = true,
        VerifiedOnly = true,
        MinRating = 4.0m,
        MinCoaches = 3,
        MinMembershipPrice = 1000m,
        MaxMembershipPrice = 10000m,
        SortBy = "HighestRated",
        Page = 1,
        PageSize = 20
    };
}

/// <summary>
/// Swagger request example for <see cref="NearbyAcademiesRequest"/>.
/// </summary>
public class NearbyAcademiesRequestExample : IExamplesProvider<NearbyAcademiesRequest>
{
    public NearbyAcademiesRequest GetExamples() => new()
    {
        Latitude = 19.0760m,
        Longitude = 72.8777m,
        RadiusKm = 10,
        Limit = 20,
        SportName = "Cricket"
    };
}

/// <summary>
/// Swagger request example for <see cref="SaveAcademySearchRequest"/>.
/// </summary>
public class SaveAcademySearchRequestExample : IExamplesProvider<SaveAcademySearchRequest>
{
    public SaveAcademySearchRequest GetExamples() => new()
    {
        SearchName = "My Cricket Search",
        SearchTerm = "cricket coaching",
        City = "Mumbai",
        State = "Maharashtra",
        Country = "India",
        SportName = "Cricket",
        HasCricketGround = true,
        HasGym = true,
        VerifiedOnly = true,
        MinRating = 3.5m,
        MinMembershipPrice = 2000m,
        MaxMembershipPrice = 8000m,
        ResultCount = 12
    };
}

#endregion

#region Academy Search Response Examples

/// <summary>
/// Swagger response example for <see cref="AcademySearchPageResultDto"/>.
/// </summary>
public class AcademySearchPageResultDtoExample : IExamplesProvider<AcademySearchPageResultDto>
{
    public AcademySearchPageResultDto GetExamples() => new()
    {
        Items =
        [
            new AcademySearchResultDto
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
                AcademyCode = "ACAD-20260725-A1B2",
                Name = "Elite Cricket Academy",
                Description = "Premier cricket training academy with world-class facilities.",
                LogoUrl = "https://cdn.sportsgurukul.com/logos/elite-cricket.png",
                BannerUrl = "https://cdn.sportsgurukul.com/banners/elite-cricket.jpg",
                Email = "info@elitecricket.com",
                Phone = "+919876543210",
                Status = "Active",
                VerificationStatus = "Verified",
                IsVerified = true,
                IsGovernmentRegistered = true,
                EstablishedDate = new DateTime(2015, 3, 15),
                YearsEstablished = 11,
                Country = "India",
                State = "Maharashtra",
                City = "Mumbai",
                District = "Andheri",
                PinCode = "400058",
                Latitude = 19.1364m,
                Longitude = 72.8296m,
                DistanceKm = 3.2,
                Sports = ["Cricket", "Football", "Fitness"],
                FacilityTypes = ["Outdoor", "Indoor"],
                FacilityNames = ["Main Cricket Ground", "Indoor nets", "Gym"],
                MinMembershipPrice = 2000m,
                MaxMembershipPrice = 8000m,
                TotalCoaches = 8,
                TotalAthletes = 150,
                TotalBranches = 2,
                TotalFacilities = 6,
                TotalMemberships = 4,
                TotalReviews = 45,
                AverageRating = 4.5m,
                ViewCount = 1200,
                IsOpenNow = true,
                IsWeekendOpen = true,
                CreatedAt = DateTime.UtcNow.AddDays(-180)
            },
            new AcademySearchResultDto
            {
                Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
                AcademyCode = "ACAD-20260725-C3D4",
                Name = "Mumbai Sports Hub",
                Description = "Multi-sport complex with cricket, football, and swimming facilities.",
                LogoUrl = "https://cdn.sportsgurukul.com/logos/mumbai-hub.png",
                BannerUrl = "https://cdn.sportsgurukul.com/banners/mumbai-hub.jpg",
                Email = "contact@mumbaisportshub.com",
                Phone = "+919876543211",
                Status = "Active",
                VerificationStatus = "Verified",
                IsVerified = true,
                IsGovernmentRegistered = true,
                EstablishedDate = new DateTime(2018, 7, 20),
                YearsEstablished = 8,
                Country = "India",
                State = "Maharashtra",
                City = "Mumbai",
                District = "Bandra",
                PinCode = "400050",
                Latitude = 19.0596m,
                Longitude = 72.8295m,
                DistanceKm = 5.8,
                Sports = ["Cricket", "Football", "Swimming", "Fitness"],
                FacilityTypes = ["Outdoor", "Indoor", "Pool"],
                FacilityNames = ["Cricket Ground", "Football Turf", "Swimming Pool", "Gym"],
                MinMembershipPrice = 3000m,
                MaxMembershipPrice = 12000m,
                TotalCoaches = 12,
                TotalAthletes = 250,
                TotalBranches = 3,
                TotalFacilities = 10,
                TotalMemberships = 6,
                TotalReviews = 78,
                AverageRating = 4.2m,
                ViewCount = 2500,
                IsOpenNow = true,
                IsWeekendOpen = true,
                CreatedAt = DateTime.UtcNow.AddDays(-120)
            }
        ],
        TotalRecords = 2,
        TotalPages = 1,
        CurrentPage = 1,
        PageSize = 20,
        AvailableSortOptions = ["Nearest", "HighestRated", "MostPopular", "Newest", "Alphabetical", "LowestMembershipCost", "MostCoaches"]
    };
}

/// <summary>
/// Swagger response example for <see cref="AcademySuggestionDto"/>.
/// </summary>
public class AcademySuggestionDtoExample : IExamplesProvider<AcademySuggestionDto>
{
    public AcademySuggestionDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        Name = "Elite Cricket Academy",
        AcademyCode = "ACAD-20260725-A1B2",
        City = "Mumbai",
        State = "Maharashtra",
        LogoUrl = "https://cdn.sportsgurukul.com/logos/elite-cricket.png",
        IsVerified = true
    };
}

/// <summary>
/// Swagger response example for <see cref="AcademySimilarDto"/>.
/// </summary>
public class AcademySimilarDtoExample : IExamplesProvider<AcademySimilarDto>
{
    public AcademySimilarDto GetExamples() => new()
    {
        Id = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        Name = "Mumbai Sports Hub",
        AcademyCode = "ACAD-20260725-C3D4",
        LogoUrl = "https://cdn.sportsgurukul.com/logos/mumbai-hub.png",
        Description = "Multi-sport complex with cricket, football, and swimming facilities.",
        City = "Mumbai",
        State = "Maharashtra",
        IsVerified = true,
        CommonSports = ["Cricket", "Football"],
        CommonFacilities = ["Gym", "Parking", "WiFi"],
        SimilarityScore = 0.85
    };
}

/// <summary>
/// Swagger response example for <see cref="SavedAcademySearchDto"/>.
/// </summary>
public class SavedAcademySearchDtoExample : IExamplesProvider<SavedAcademySearchDto>
{
    public SavedAcademySearchDto GetExamples() => new()
    {
        Id = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
        SearchName = "Mumbai Cricket Academies",
        SearchTerm = "cricket",
        City = "Mumbai",
        State = "Maharashtra",
        SportName = "Cricket",
        VerifiedOnly = true,
        MinMembershipPrice = 2000m,
        MaxMembershipPrice = 10000m,
        ResultCount = 12,
        CreatedAt = DateTime.UtcNow.AddDays(-5)
    };
}

/// <summary>
/// Swagger response example for <see cref="RecentAcademySearchDto"/>.
/// </summary>
public class RecentAcademySearchDtoExample : IExamplesProvider<RecentAcademySearchDto>
{
    public RecentAcademySearchDto GetExamples() => new()
    {
        Id = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
        SearchTerm = "football coaching",
        City = "Pune",
        State = "Maharashtra",
        SportName = "Football",
        AcademyCount = 8,
        SearchedAt = DateTime.UtcNow.AddHours(-2)
    };
}

/// <summary>
/// Swagger response example for <see cref="PopularAcademyDto"/>.
/// </summary>
public class PopularAcademyDtoExample : IExamplesProvider<PopularAcademyDto>
{
    public PopularAcademyDto GetExamples() => new()
    {
        Id = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        Name = "Elite Cricket Academy",
        AcademyCode = "ACAD-20260725-A1B2",
        LogoUrl = "https://cdn.sportsgurukul.com/logos/elite-cricket.png",
        City = "Mumbai",
        State = "Maharashtra",
        IsVerified = true,
        ViewCount = 1200,
        AverageRating = 4.5m
    };
}

#endregion
