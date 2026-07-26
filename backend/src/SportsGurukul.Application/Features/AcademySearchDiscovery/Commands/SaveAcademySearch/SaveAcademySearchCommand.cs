using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Commands.SaveAcademySearch;

public class SaveAcademySearchCommand : IRequest<Result<SavedAcademySearchDto>>
{
    public Guid UserId { get; set; }
    public string SearchName { get; set; } = string.Empty;
    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? District { get; set; }
    public string? PinCode { get; set; }
    public string? SportName { get; set; }
    public string? SportCategory { get; set; }
    public string? FacilityType { get; set; }
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
    public bool? OpenNow { get; set; }
    public bool? WeekendOpen { get; set; }
    public decimal? MinMembershipPrice { get; set; }
    public decimal? MaxMembershipPrice { get; set; }
    public decimal? MinRating { get; set; }
    public int ResultCount { get; set; }
}
