using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.NearbyEvents;

public class NearbyEventsQuery : IRequest<Result<IReadOnlyList<NearbyEventDto>>>
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal RadiusKm { get; set; } = 10;
    public int Limit { get; set; } = 20;
}
