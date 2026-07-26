using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetRecentAcademySearches;

public class GetRecentAcademySearchesQuery : IRequest<Result<IReadOnlyList<RecentAcademySearchDto>>>
{
    public Guid UserId { get; set; }
    public int Limit { get; set; } = 10;
}
