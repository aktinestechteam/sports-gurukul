using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

namespace SportsGurukul.Application.Features.EventSearchDiscovery.Queries.Autocomplete;

public class AutocompleteQueryHandler : IRequestHandler<AutocompleteQuery, Result<IReadOnlyList<EventAutocompleteSuggestionDto>>>
{
    private readonly IEventSearchRepository _searchRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<AutocompleteQueryHandler> _logger;
    private const string CachePrefix = "autocomplete_";
    private const int MinPrefixLength = 2;

    public AutocompleteQueryHandler(
        IEventSearchRepository searchRepository,
        ICacheService cacheService,
        ILogger<AutocompleteQueryHandler> logger)
    {
        _searchRepository = searchRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<EventAutocompleteSuggestionDto>>> Handle(
        AutocompleteQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prefix) || request.Prefix.Length < MinPrefixLength)
        {
            return Result<IReadOnlyList<EventAutocompleteSuggestionDto>>.Success([]);
        }

        var cacheKey = $"{CachePrefix}{request.Prefix.ToLowerInvariant()}_{request.Limit}";
        var cached = await _cacheService.GetAsync<List<EventAutocompleteSuggestionDto>>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            return Result<IReadOnlyList<EventAutocompleteSuggestionDto>>.Success(cached);
        }

        _logger.LogInformation("Autocomplete: prefix='{Prefix}', limit={Limit}", request.Prefix, request.Limit);

        var results = await _searchRepository.GetAutocompleteSuggestionsAsync(
            request.Prefix, request.Limit, cancellationToken);

        var suggestions = results.Select(r => new EventAutocompleteSuggestionDto
        {
            Id = r.Id,
            Text = r.Text,
            Type = r.Type,
            SubText = r.SubText,
            EventType = r.EventType,
            EventDate = r.EventDate,
            Highlight = HighlightMatch(r.Text, request.Prefix)
        }).ToList();

        await _cacheService.SetAsync(cacheKey, suggestions, TimeSpan.FromMinutes(5), cancellationToken);

        return Result<IReadOnlyList<EventAutocompleteSuggestionDto>>.Success(suggestions);
    }

    private static string HighlightMatch(string text, string prefix)
    {
        var index = text.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);
        if (index < 0) return text;

        return $"{text[..index]}**{text[index..(index + prefix.Length)]}**{text[(index + prefix.Length)..]}";
    }
}
