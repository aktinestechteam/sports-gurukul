using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachSuggestions;

public class GetCoachSuggestionsQuery : IRequest<Result<IReadOnlyList<CoachSearchSuggestionDto>>>
{
    public string Prefix { get; set; } = string.Empty;
    public int Limit { get; set; } = 10;
}
