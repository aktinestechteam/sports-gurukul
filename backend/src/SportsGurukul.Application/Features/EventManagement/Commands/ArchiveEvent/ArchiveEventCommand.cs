using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.ArchiveEvent;

public class ArchiveEventCommand : IRequest<Result<EventDto>>
{
    public Guid EventId { get; set; }
}
