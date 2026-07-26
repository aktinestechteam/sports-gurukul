using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademyManagement.DTOs;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.TransferAthlete;

public class TransferAthleteCommand : IRequest<Result<AcademyAthleteSummaryDto>>
{
    public Guid FromAcademyId { get; set; }
    public Guid ToAcademyId { get; set; }
    public Guid AthleteId { get; set; }
}
