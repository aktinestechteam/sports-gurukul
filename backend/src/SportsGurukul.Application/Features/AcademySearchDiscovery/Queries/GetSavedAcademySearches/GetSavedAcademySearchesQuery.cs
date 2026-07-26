using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetSavedAcademySearches;

public class GetSavedAcademySearchesQuery : IRequest<Result<IReadOnlyList<SavedAcademySearchDto>>>
{
    public Guid UserId { get; set; }
}
