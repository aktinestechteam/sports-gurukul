using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;
using SportsGurukul.Application.Features.EventSearchDiscovery.Queries.SearchEvents;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

public class EventSearchRequestExample : IExamplesProvider<SearchEventsQuery>
{
    public SearchEventsQuery GetExamples() => new()
    {
        SearchTerm = "cricket tournament",
        City = "Mumbai",
        EventType = "Competition",
        DateFrom = DateTime.UtcNow,
        DateTo = DateTime.UtcNow.AddDays(30),
        MinPrice = 0,
        MaxPrice = 5000,
        SortBy = "Upcoming",
        Page = 1,
        PageSize = 20
    };
}

public class EventSearchResponseExample : IExamplesProvider<ApiResponse<EventSearchPageResultDto>>
{
    public ApiResponse<EventSearchPageResultDto> GetExamples() => new()
    {
        Success = true,
        Message = "Events retrieved successfully.",
        Data = new EventSearchPageResultDto
        {
            Items = new List<EventCardDto>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    EventCode = "EVT-2026-001",
                    EventName = "Mumbai Cricket Championship 2026",
                    ShortDescription = "Annual cricket championship for under-18 athletes",
                    BannerUrl = "https://storage.sportsgurukul.com/events/cricket-championship-banner.jpg",
                    EventType = "Competition",
                    Status = "RegistrationOpen",
                    StartDate = DateTime.UtcNow.AddDays(15),
                    EndDate = DateTime.UtcNow.AddDays(17),
                    RegistrationCloseDate = DateTime.UtcNow.AddDays(10),
                    MaxParticipants = 200,
                    RegistrationFee = 1500,
                    IsFeatured = true,
                    AcademyName = "Mumbai Sports Academy",
                    SportName = "Cricket",
                    City = "Mumbai",
                    State = "Maharashtra",
                    AverageRating = 4.5m,
                    TotalReviews = 48,
                    DaysUntilStart = 15,
                    IsRegistrationOpen = true,
                    IsSoldOut = false
                }
            },
            TotalRecords = 45,
            TotalPages = 3,
            CurrentPage = 1,
            PageSize = 20,
            SearchTimeMs = 85.3
        }
    };
}

public class UpcomingEventsResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<EventCardDto>>>
{
    public ApiResponse<IReadOnlyList<EventCardDto>> GetExamples() => new()
    {
        Success = true,
        Message = "Upcoming events retrieved successfully.",
        Data = new List<EventCardDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EventCode = "EVT-2026-010",
                EventName = "Football Training Camp",
                ShortDescription = "3-day intensive football training",
                EventType = "Camp",
                Status = "RegistrationOpen",
                StartDate = DateTime.UtcNow.AddDays(5),
                EndDate = DateTime.UtcNow.AddDays(7),
                RegistrationFee = 2000,
                AcademyName = "Elite Football Academy",
                SportName = "Football",
                City = "Delhi",
                IsRegistrationOpen = true,
                DaysUntilStart = 5
            }
        }
    };
}

public class NearbyEventsResponseExample : IExamplesProvider<ApiResponse<IReadOnlyList<NearbyEventDto>>>
{
    public ApiResponse<IReadOnlyList<NearbyEventDto>> GetExamples() => new()
    {
        Success = true,
        Message = "Nearby events retrieved successfully.",
        Data = new List<NearbyEventDto>
        {
            new()
            {
                Id = Guid.NewGuid(),
                EventName = "Badminton Workshop",
                EventCode = "EVT-2026-020",
                ShortDescription = "Learn badminton techniques from professional coaches",
                EventType = "Workshop",
                StartDate = DateTime.UtcNow.AddDays(3),
                RegistrationFee = 500,
                AcademyName = "City Sports Center",
                City = "Pune",
                Latitude = 18.5204m,
                Longitude = 73.8567m,
                DistanceKm = 2.5,
                IsRegistrationOpen = true
            }
        }
    };
}
