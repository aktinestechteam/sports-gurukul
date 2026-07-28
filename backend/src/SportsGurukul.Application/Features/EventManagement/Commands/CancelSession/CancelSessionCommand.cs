using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.CancelSession;

public class CancelSessionCommand : IRequest<Result<EventSessionDto>>
{
    public Guid SessionId { get; set; }
}
