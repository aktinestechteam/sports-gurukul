using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeactivateCoach;

public class DeactivateCoachCommand : IRequest<Result<CoachDto>>
{
    public Guid CoachId { get; set; }
    public string? Reason { get; set; }
}
