using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.TrendingEvents;

public class TrendingEventsQuery : IRequest<Result<IReadOnlyList<TrendingEventDto>>>
{
    public string? City { get; set; }
    public int Limit { get; set; } = 20;
}
