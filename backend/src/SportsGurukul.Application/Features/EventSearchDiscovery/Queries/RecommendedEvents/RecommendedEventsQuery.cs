using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.RecommendedEvents;

public class RecommendedEventsQuery : IRequest<Result<IReadOnlyList<RecommendationDto>>>
{
    public Guid? UserId { get; set; }
    public string? City { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public IReadOnlyList<string>? PreferredSports { get; set; }
    public IReadOnlyList<string>? PreferredEventTypes { get; set; }
    public int Limit { get; set; } = 20;
}
