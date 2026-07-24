using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RemoveAthlete;

public class RemoveAthleteCommand : IRequest<Result<Unit>>
{
    public Guid CoachId { get; set; }
    public Guid AthleteId { get; set; }
}
