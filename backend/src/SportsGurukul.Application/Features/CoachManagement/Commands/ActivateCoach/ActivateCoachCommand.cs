using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.ActivateCoach;

public class ActivateCoachCommand : IRequest<Result<CoachDto>>
{
    public Guid CoachId { get; set; }
}
