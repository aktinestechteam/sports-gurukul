using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademyManagement.Commands.RemoveAthlete;

public class RemoveAthleteCommand : IRequest<Result<Unit>>
{
    public Guid AcademyId { get; set; }
    public Guid AthleteId { get; set; }
}
