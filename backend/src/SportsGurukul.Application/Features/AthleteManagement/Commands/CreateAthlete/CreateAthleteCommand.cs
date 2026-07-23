using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.CreateAthlete;

public class CreateAthleteCommand : IRequest<Result<AthleteDto>>
{
    public Guid UserId { get; set; }
    public AthleteLevel CurrentLevel { get; set; } = AthleteLevel.Beginner;
    public int ExperienceYears { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public BloodGroup? BloodGroup { get; set; }
    public DominantHand? DominantHand { get; set; }
    public DominantFoot? DominantFoot { get; set; }
    public string? Biography { get; set; }
}
