using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.AdvancedSearchAcademies;

public class AdvancedSearchAcademiesQuery : IRequest<Result<AcademySearchPageResultDto>>
{
    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? AcademyCode { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Country { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? District { get; set; }
    public string? PinCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? RadiusKm { get; set; }
    public string? SportName { get; set; }
    public string? SportCategory { get; set; }
    public bool? HasSwimmingPool { get; set; }
    public bool? HasIndoorStadium { get; set; }
    public bool? HasCricketGround { get; set; }
    public bool? HasFootballGround { get; set; }
    public bool? HasGym { get; set; }
    public bool? HasYogaHall { get; set; }
    public bool? HasParking { get; set; }
    public bool? HasMedicalRoom { get; set; }
    public bool? HasWifi { get; set; }
    public bool? HasCafeteria { get; set; }
    public bool? VerifiedOnly { get; set; }
    public bool? GovernmentRegisteredOnly { get; set; }
    public int? MinEstablishmentYears { get; set; }
    public decimal? MinMembershipPrice { get; set; }
    public decimal? MaxMembershipPrice { get; set; }
    public decimal? MinRating { get; set; }
    public int? MinCoaches { get; set; }
    public int? MinAthletes { get; set; }
    public int? MinBranches { get; set; }
    public bool? OpenNow { get; set; }
    public bool? WeekendOpen { get; set; }
    public string? SortBy { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
