using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.UpcomingEvents;

public class UpcomingEventsQuery : IRequest<Result<IReadOnlyList<EventCardDto>>>
{
    public string? City { get; set; }
    public Guid? AcademyId { get; set; }
    public int Limit { get; set; } = 20;
}
