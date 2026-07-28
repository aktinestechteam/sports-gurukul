using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.AssignVenue;

public class AssignVenueCommand : IRequest<Result<EventSessionDto>>
{
    public Guid SessionId { get; set; }
    public Guid VenueId { get; set; }
}
