using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class SearchService : ISearchService
{
    private readonly ILogger<SearchService> _logger;
    private readonly ITemplateManagementService _templateManagementService;
    private readonly ICampaignManagementService _campaignManagementService;
    private readonly IAnalyticsService _analyticsService;
    private readonly IAudienceSegmentationService _audienceSegmentationService;

    private readonly ConcurrentDictionary<Guid, SearchDocument> _templateIndex = new();
    private readonly ConcurrentDictionary<Guid, SearchDocument> _campaignIndex = new();
    private readonly ConcurrentDictionary<Guid, SearchDocument> _notificationIndex = new();
    private readonly ConcurrentDictionary<Guid, SearchDocument> _analyticsIndex = new();
    private readonly ConcurrentDictionary<Guid, SearchDocument> _segmentIndex = new();

    private static readonly ConcurrentDictionary<SearchEntityType, int> IndexSizes = new();

    private record SearchDocument(
        Guid Id,
        SearchEntityType EntityType,
        string Title,
        string? Description,
        string? Content,
        string? Status,
        string? Channel,
        DateTime? CreatedAt,
        DateTime? UpdatedAt,
        object OriginalEntity
    );

    public SearchService(
        ILogger<SearchService> logger,
        ITemplateManagementService templateManagementService,
        ICampaignManagementService campaignManagementService,
        IAnalyticsService analyticsService,
        IAudienceSegmentationService audienceSegmentationService)
    {
        _logger = logger;
        _templateManagementService = templateManagementService;
        _campaignManagementService = campaignManagementService;
        _analyticsService = analyticsService;
        _audienceSegmentationService = audienceSegmentationService;
    }

    public Task<UnifiedSearchResult> SearchAsync(UnifiedSearchRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var query = (request.Query ?? string.Empty).Trim();
        var indices = GetTargetIndices(request.EntityType);

        var scoredResults = new List<(SearchDocument Doc, double Score, List<string> MatchedFields)>();

        foreach (var (_, doc) in indices.SelectMany(idx => idx))
        {
            var (score, matchedFields) = ComputeRelevance(doc, query);
            if (score > 0)
                scoredResults.Add((doc, score, matchedFields));
        }

        var sorted = request.SortDescending
            ? scoredResults.OrderByDescending(r => r.Score).ThenBy(r => r.Doc.Title).ToList()
            : scoredResults.OrderBy(r => r.Score).ThenBy(r => r.Doc.Title).ToList();

        var totalResults = sorted.Count;
        var pageNumber = Math.Max(1, request.PageNumber);
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var paged = sorted
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => MapToResultItem(r.Doc, r.Score, r.MatchedFields))
            .ToList();

        sw.Stop();

        return Task.FromResult(new UnifiedSearchResult(
            request.Query,
            request.EntityType,
            totalResults,
            pageNumber,
            pageSize,
            (pageNumber * pageSize) < totalResults,
            sw.ElapsedMilliseconds,
            paged
        ));
    }

    public Task<List<SearchSuggestionDto>> GetSuggestionsAsync(string query, SearchEntityType entityType, int maxResults = 10, CancellationToken ct = default)
    {
        var q = (query ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(q))
            return Task.FromResult(new List<SearchSuggestionDto>());

        var indices = GetTargetIndices(entityType);
        var suggestions = new Dictionary<string, (SearchEntityType Type, double Score, int Count)>();

        foreach (var (_, doc) in indices.SelectMany(idx => idx))
        {
            var title = doc.Title.ToLowerInvariant();
            if (!title.StartsWith(q))
                continue;

            var score = title == q ? 100.0 :
                        title.Length == q.Length ? 90.0 :
                        80.0 - (title.Length - q.Length) * 0.5;

            var key = doc.Title;
            if (suggestions.TryGetValue(key, out var existing))
            {
                suggestions[key] = (existing.Type, Math.Max(existing.Score, score), existing.Count + 1);
            }
            else
            {
                suggestions[key] = (doc.EntityType, score, 1);
            }
        }

        var results = suggestions
            .OrderByDescending(s => s.Value.Score)
            .ThenBy(s => s.Key)
            .Take(maxResults)
            .Select(s => new SearchSuggestionDto(s.Key, s.Value.Type, s.Value.Count, s.Value.Score))
            .ToList();

        return Task.FromResult(results);
    }

    public Task<List<SearchFacetDto>> GetFacetsAsync(string query, SearchEntityType entityType, CancellationToken ct = default)
    {
        var q = (query ?? string.Empty).Trim().ToLowerInvariant();
        var indices = GetTargetIndices(entityType);
        var allDocs = indices.SelectMany(idx => idx.Values).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            allDocs = allDocs.Where(d =>
                d.Title.ToLowerInvariant().Contains(q) ||
                (d.Description?.ToLowerInvariant().Contains(q) ?? false) ||
                (d.Content?.ToLowerInvariant().Contains(q) ?? false));
        }

        var docs = allDocs.ToList();

        var statusFacet = new SearchFacetDto("Status",
            docs.GroupBy(d => d.Status ?? "Unknown")
                .Select(g => new SearchFacetValueDto(g.Key, g.Count()))
                .OrderByDescending(v => v.Count)
                .ToList()
        );

        var channelFacet = new SearchFacetDto("Channel",
            docs.GroupBy(d => d.Channel ?? "Unknown")
                .Select(g => new SearchFacetValueDto(g.Key, g.Count()))
                .OrderByDescending(v => v.Count)
                .ToList()
        );

        var typeFacet = new SearchFacetDto("EntityType",
            docs.GroupBy(d => d.EntityType.ToString())
                .Select(g => new SearchFacetValueDto(g.Key, g.Count()))
                .OrderByDescending(v => v.Count)
                .ToList()
        );

        return Task.FromResult(new List<SearchFacetDto> { statusFacet, channelFacet, typeFacet });
    }

    public Task<UnifiedSearchResult> SearchTemplatesAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default)
    {
        return SearchByEntityTypeAsync(query, SearchEntityType.Template, filters, page, size, ct);
    }

    public Task<UnifiedSearchResult> SearchCampaignsAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default)
    {
        return SearchByEntityTypeAsync(query, SearchEntityType.Campaign, filters, page, size, ct);
    }

    public Task<UnifiedSearchResult> SearchNotificationsAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default)
    {
        return SearchByEntityTypeAsync(query, SearchEntityType.Notification, filters, page, size, ct);
    }

    public Task<UnifiedSearchResult> SearchAnalyticsAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default)
    {
        return SearchByEntityTypeAsync(query, SearchEntityType.Analytics, filters, page, size, ct);
    }

    public Task<UnifiedSearchResult> SearchSegmentsAsync(string query, Dictionary<string, string>? filters, int page, int size, CancellationToken ct = default)
    {
        return SearchByEntityTypeAsync(query, SearchEntityType.Segment, filters, page, size, ct);
    }

    public Task IndexEntityAsync<T>(T entity, CancellationToken ct = default)
    {
        if (entity == null)
            return Task.CompletedTask;

        SearchDocument doc;
        ConcurrentDictionary<Guid, SearchDocument> targetIndex;

        switch (entity)
        {
            case TemplateDetailDto t:
                doc = new SearchDocument(t.Id, SearchEntityType.Template, t.Name, t.Description, t.SubjectTemplate, t.Status.ToString(), t.ChannelType.ToString(), t.CreatedAt, null, t);
                targetIndex = _templateIndex;
                break;
            case CampaignDetailDto c:
                doc = new SearchDocument(c.Id, SearchEntityType.Campaign, c.Name, c.Description, null, c.Status.ToString(), c.ChannelType.ToString(), c.CreatedAt, null, c);
                targetIndex = _campaignIndex;
                break;
            case CampaignPerformanceDto cp:
                doc = new SearchDocument(cp.CampaignId, SearchEntityType.Campaign, cp.CampaignName, null, null, null, null, null, null, cp);
                targetIndex = _campaignIndex;
                break;
            case ProviderPerformanceDto pp:
                doc = new SearchDocument(pp.ProviderId, SearchEntityType.Provider, pp.ProviderName, pp.ProviderType, null, null, pp.ProviderType, pp.PeriodStart, pp.PeriodEnd, pp);
                targetIndex = _analyticsIndex;
                break;
            case SegmentDefinitionDto sd:
                doc = new SearchDocument(sd.Id, SearchEntityType.Segment, sd.Name, sd.Description, null, null, null, sd.CreatedAt, sd.UpdatedAt, sd);
                targetIndex = _segmentIndex;
                break;
            case TemplatePerformanceSummaryDto tp:
                doc = new SearchDocument(tp.Id, SearchEntityType.Template, tp.Name, null, null, null, tp.ChannelType.ToString(), null, tp.UpdatedAt, tp);
                targetIndex = _templateIndex;
                break;
            case ChannelPerformanceDto cp:
                doc = new SearchDocument(Guid.NewGuid(), SearchEntityType.Analytics, cp.ChannelName, null, null, null, cp.ChannelType.ToString(), cp.PeriodStart, cp.PeriodEnd, cp);
                targetIndex = _analyticsIndex;
                break;
            default:
                _logger.LogWarning("IndexEntityAsync: unsupported entity type {Type}", typeof(T).Name);
                return Task.CompletedTask;
        }

        targetIndex[doc.Id] = doc;
        IndexSizes.AddOrUpdate(doc.EntityType, 1, (_, v) => v + 1);
        return Task.CompletedTask;
    }

    public Task RebuildIndexAsync(SearchEntityType entityType, CancellationToken ct = default)
    {
        switch (entityType)
        {
            case SearchEntityType.Template:
                _templateIndex.Clear();
                break;
            case SearchEntityType.Campaign:
                _campaignIndex.Clear();
                break;
            case SearchEntityType.Notification:
                _notificationIndex.Clear();
                break;
            case SearchEntityType.Analytics:
                _analyticsIndex.Clear();
                break;
            case SearchEntityType.Segment:
                _segmentIndex.Clear();
                break;
            case SearchEntityType.All:
                ClearAllIndices();
                break;
        }

        IndexSizes.TryRemove(entityType, out _);
        _logger.LogInformation("Rebuilt search index for {EntityType}", entityType);
        return Task.CompletedTask;
    }

    public Task ClearIndexAsync(SearchEntityType entityType, CancellationToken ct = default)
    {
        return RebuildIndexAsync(entityType, ct);
    }

    private Task<UnifiedSearchResult> SearchByEntityTypeAsync(string query, SearchEntityType entityType, Dictionary<string, string>? filters, int page, int size, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var q = (query ?? string.Empty).Trim().ToLowerInvariant();
        var index = GetSingleIndex(entityType);

        var docs = index.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(q))
        {
            docs = docs.Where(d =>
                d.Title.ToLowerInvariant().Contains(q) ||
                (d.Description?.ToLowerInvariant().Contains(q) ?? false) ||
                (d.Content?.ToLowerInvariant().Contains(q) ?? false));
        }

        if (filters != null)
        {
            foreach (var (key, value) in filters)
            {
                var filterVal = value?.ToLowerInvariant();
                docs = key.ToLowerInvariant() switch
                {
                    "status" => docs.Where(d => (d.Status ?? string.Empty).Equals(filterVal, StringComparison.OrdinalIgnoreCase)),
                    "channel" => docs.Where(d => (d.Channel ?? string.Empty).Equals(filterVal, StringComparison.OrdinalIgnoreCase)),
                    "type" => docs.Where(d => d.EntityType.ToString().Equals(filterVal, StringComparison.OrdinalIgnoreCase)),
                    _ => docs
                };
            }
        }

        var scoredResults = docs.Select(d =>
        {
            var (score, matchedFields) = ComputeRelevance(d, q);
            return (Doc: d, Score: score, Fields: matchedFields);
        }).ToList();

        if (!string.IsNullOrWhiteSpace(q))
            scoredResults = scoredResults.Where(r => r.Score > 0).ToList();

        var sorted = scoredResults
            .OrderByDescending(r => r.Score)
            .ThenBy(r => r.Doc.Title)
            .ToList();

        var totalResults = sorted.Count;
        var pageNumber = Math.Max(1, page);
        var pageSize = Math.Clamp(size, 1, 100);
        var paged = sorted
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(r => MapToResultItem(r.Doc, r.Score, r.Fields))
            .ToList();

        sw.Stop();

        return Task.FromResult(new UnifiedSearchResult(
            query,
            entityType,
            totalResults,
            pageNumber,
            pageSize,
            (pageNumber * pageSize) < totalResults,
            sw.ElapsedMilliseconds,
            paged
        ));
    }

    private static (double Score, List<string> MatchedFields) ComputeRelevance(SearchDocument doc, string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return (0.5, []);

        var q = query.ToLowerInvariant();
        var title = doc.Title.ToLowerInvariant();
        var description = (doc.Description ?? string.Empty).ToLowerInvariant();
        var content = (doc.Content ?? string.Empty).ToLowerInvariant();

        var score = 0.0;
        var matchedFields = new List<string>();

        if (title == q)
        {
            score += 100;
            matchedFields.Add("Title (exact)");
        }
        else if (title.StartsWith(q))
        {
            score += 80;
            matchedFields.Add("Title (prefix)");
        }
        else if (title.Contains(q))
        {
            score += 60;
            matchedFields.Add("Title (contains)");
        }

        if (!string.IsNullOrEmpty(description))
        {
            if (description == q)
            {
                score += 50;
                matchedFields.Add("Description (exact)");
            }
            else if (description.StartsWith(q))
            {
                score += 40;
                matchedFields.Add("Description (prefix)");
            }
            else if (description.Contains(q))
            {
                score += 30;
                matchedFields.Add("Description (contains)");
            }
        }

        if (!string.IsNullOrEmpty(content))
        {
            if (content == q)
            {
                score += 25;
                matchedFields.Add("Content (exact)");
            }
            else if (content.Contains(q))
            {
                score += 15;
                matchedFields.Add("Content (contains)");
            }
        }

        if (doc.Status != null && doc.Status.Equals(q, StringComparison.OrdinalIgnoreCase))
        {
            score += 10;
            matchedFields.Add("Status");
        }

        if (doc.Channel != null && doc.Channel.Equals(q, StringComparison.OrdinalIgnoreCase))
        {
            score += 10;
            matchedFields.Add("Channel");
        }

        return (score, matchedFields);
    }

    private static SearchResultItemDto MapToResultItem(SearchDocument doc, double score, List<string> matchedFields)
    {
        return new SearchResultItemDto(
            doc.Id,
            doc.EntityType,
            doc.Title,
            doc.Description,
            doc.Content?.Length > 200 ? doc.Content[..200] : doc.Content,
            doc.Status,
            doc.Channel,
            Math.Round(score, 1),
            matchedFields,
            doc.CreatedAt,
            doc.UpdatedAt,
            null
        );
    }

    private List<ConcurrentDictionary<Guid, SearchDocument>> GetTargetIndices(SearchEntityType entityType)
    {
        return entityType switch
        {
            SearchEntityType.Template => [_templateIndex],
            SearchEntityType.Campaign => [_campaignIndex],
            SearchEntityType.Notification => [_notificationIndex],
            SearchEntityType.Analytics => [_analyticsIndex],
            SearchEntityType.Segment => [_segmentIndex],
            SearchEntityType.Provider => [_analyticsIndex],
            SearchEntityType.Schedule => [_campaignIndex],
            SearchEntityType.All => [_templateIndex, _campaignIndex, _notificationIndex, _analyticsIndex, _segmentIndex],
            _ => []
        };
    }

    private ConcurrentDictionary<Guid, SearchDocument> GetSingleIndex(SearchEntityType entityType)
    {
        return entityType switch
        {
            SearchEntityType.Template => _templateIndex,
            SearchEntityType.Campaign => _campaignIndex,
            SearchEntityType.Notification => _notificationIndex,
            SearchEntityType.Analytics or SearchEntityType.Provider => _analyticsIndex,
            SearchEntityType.Segment => _segmentIndex,
            SearchEntityType.Schedule => _campaignIndex,
            _ => _templateIndex
        };
    }

    private void ClearAllIndices()
    {
        _templateIndex.Clear();
        _campaignIndex.Clear();
        _notificationIndex.Clear();
        _analyticsIndex.Clear();
        _segmentIndex.Clear();
    }
}
