using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CancelEvent;

public class CancelEventCommand : IRequest<Result<EventDto>>
{
    public Guid EventId { get; set; }
    public string? Reason { get; set; }
}
