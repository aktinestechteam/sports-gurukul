using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetAssignedAthletes;

public class GetAssignedAthletesQuery : IRequest<Result<IReadOnlyList<AssignedAthleteDto>>>
{
    public Guid CoachId { get; set; }
}
