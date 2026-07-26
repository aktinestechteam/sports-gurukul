using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSimilarAcademies;

public class GetSimilarAcademiesQuery : IRequest<Result<IReadOnlyList<AcademySimilarDto>>>
{
    public Guid AcademyId { get; set; }
    public int Limit { get; set; } = 5;
}
