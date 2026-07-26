using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularSearchTerms;

public class GetPopularSearchTermsQuery : IRequest<Result<IReadOnlyList<string>>>
{
    public int Limit { get; set; } = 10;
}
