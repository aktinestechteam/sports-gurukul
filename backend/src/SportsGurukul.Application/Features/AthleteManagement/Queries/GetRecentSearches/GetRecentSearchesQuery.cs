using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetRecentSearches;

public class GetRecentSearchesQuery : IRequest<Result<IReadOnlyList<RecentSearchDto>>>
{
    public Guid UserId { get; set; }
    public int Limit { get; set; } = 10;
}
