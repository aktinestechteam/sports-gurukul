using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;

namespace SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSuggestions;

public class GetAthleteSuggestionsQuery : IRequest<Result<IReadOnlyList<AthleteSearchSuggestionDto>>>
{
    public string Prefix { get; set; } = string.Empty;
    public int Limit { get; set; } = 10;
}
