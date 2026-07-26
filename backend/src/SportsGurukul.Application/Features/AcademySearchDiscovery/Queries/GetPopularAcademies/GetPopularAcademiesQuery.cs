using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetPopularAcademies;

public class GetPopularAcademiesQuery : IRequest<Result<IReadOnlyList<PopularAcademyDto>>>
{
    public int Limit { get; set; } = 10;
}
