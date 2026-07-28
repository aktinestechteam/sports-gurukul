using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventManagement.DTOs;

namespace SportsGurukul.Application.Features.EventManagement.Commands.AssignCoach;

public class AssignCoachCommand : IRequest<Result<EventSessionDto>>
{
    public Guid SessionId { get; set; }
    public Guid CoachId { get; set; }
}
