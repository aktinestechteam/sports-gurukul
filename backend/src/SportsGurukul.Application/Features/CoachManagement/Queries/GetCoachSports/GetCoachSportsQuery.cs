using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachSports;

public class GetCoachSportsQuery : IRequest<Result<IReadOnlyList<SportDto>>>
{
    public Guid CoachId { get; set; }
}
