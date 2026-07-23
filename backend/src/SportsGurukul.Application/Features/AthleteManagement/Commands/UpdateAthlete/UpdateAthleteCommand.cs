using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAthlete;

public class UpdateAthleteCommand : IRequest<Result<AthleteDto>>
{
    public Guid AthleteId { get; set; }
    public AthleteLevel? CurrentLevel { get; set; }
    public int? ExperienceYears { get; set; }
    public string? Height { get; set; }
    public string? Weight { get; set; }
    public BloodGroup? BloodGroup { get; set; }
    public DominantHand? DominantHand { get; set; }
    public DominantFoot? DominantFoot { get; set; }
    public string? Biography { get; set; }
    public AthleteStatus? Status { get; set; }
}
