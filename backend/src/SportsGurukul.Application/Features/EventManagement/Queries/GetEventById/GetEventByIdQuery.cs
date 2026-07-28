using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetEventById;

public class GetEventByIdQuery : IRequest<Result<EventDto>>
{
    public Guid EventId { get; set; }
}
