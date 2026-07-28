using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.FeaturedEvents;

public class FeaturedEventsQuery : IRequest<Result<IReadOnlyList<FeaturedEventDto>>>
{
    public string? City { get; set; }
    public string? SportName { get; set; }
    public int Limit { get; set; } = 20;
}
