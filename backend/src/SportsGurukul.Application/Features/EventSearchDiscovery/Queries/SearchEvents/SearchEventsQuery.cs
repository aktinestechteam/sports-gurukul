using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.SearchEvents;

public class SearchEventsQuery : IRequest<Result<EventSearchPageResultDto>>
{
    public string? SearchTerm { get; set; }
    public Guid? SportId { get; set; }
    public Guid? AcademyId { get; set; }
    public Guid? CoachId { get; set; }
    public string? EventType { get; set; }
    public string? Category { get; set; }
    public string? SkillLevel { get; set; }
    public string? AgeGroup { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinRating { get; set; }
    public string? Language { get; set; }
    public string? Availability { get; set; }
    public string? RegistrationStatus { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public decimal? RadiusKm { get; set; }
}
