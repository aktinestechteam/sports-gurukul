using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Commands.TrackRecentlyViewed;

public class TrackRecentlyViewedCommand : IRequest<Result<bool>>
{
    public Guid EventId { get; set; }
    public Guid? UserId { get; set; }
    public string? Source { get; set; }
    public string? DeviceType { get; set; }
}
