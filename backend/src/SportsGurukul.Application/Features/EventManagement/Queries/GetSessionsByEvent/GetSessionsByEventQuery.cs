using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Queries.GetSessionsByEvent;

public class GetSessionsByEventQuery : IRequest<Result<List<EventSessionDto>>>
{
    public Guid EventId { get; set; }
}
