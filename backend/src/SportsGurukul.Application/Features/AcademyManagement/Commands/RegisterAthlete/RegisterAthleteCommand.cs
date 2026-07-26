using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RegisterAthlete;

public class RegisterAthleteCommand : IRequest<Result<AcademyAthleteSummaryDto>>
{
    public Guid AcademyId { get; set; }
    public Guid AthleteId { get; set; }
}
