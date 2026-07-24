using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetRecentCoachSearches;

public class GetRecentCoachSearchesQuery : IRequest<Result<IReadOnlyList<RecentSearchDto>>>
{
    public Guid UserId { get; set; }
    public int Limit { get; set; } = 10;
}
