using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AcademySearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.AcademySearchDiscovery.Queries.GetAcademySuggestions;

public class GetAcademySuggestionsQuery : IRequest<Result<IReadOnlyList<AcademySuggestionDto>>>
{
    public string Prefix { get; set; } = string.Empty;
    public int Limit { get; set; } = 10;
}
