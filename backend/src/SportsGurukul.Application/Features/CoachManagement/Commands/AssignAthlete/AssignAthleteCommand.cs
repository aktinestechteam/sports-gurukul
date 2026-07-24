using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.AssignAthlete;

public class AssignAthleteCommand : IRequest<Result<AssignedAthleteDto>>
{
    public Guid CoachId { get; set; }
    public Guid AthleteId { get; set; }
}
