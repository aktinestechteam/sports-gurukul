using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.Autocomplete;

public class AutocompleteQuery : IRequest<Result<IReadOnlyList<EventAutocompleteSuggestionDto>>>
{
    public string Prefix { get; set; } = string.Empty;
    public int Limit { get; set; } = 10;
}
