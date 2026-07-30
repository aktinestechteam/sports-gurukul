using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class AudienceSegmentationService : IAudienceSegmentationService
{
    private readonly ILogger<AudienceSegmentationService> _logger;
    private readonly ICacheService _cache;
    private readonly ConcurrentDictionary<Guid, SegmentDefinitionDto> _segments = new();
    private readonly ConcurrentDictionary<Guid, SavedSegmentDto> _savedSegments = new();
    private readonly ConcurrentDictionary<string, List<string>> _predefinedSegments = new();

    private static readonly TimeSpan DefaultCacheTtl = TimeSpan.FromMinutes(5);
    private static readonly Random Rng = new(42);

    public AudienceSegmentationService(ILogger<AudienceSegmentationService> logger, ICacheService cache)
    {
        _logger = logger;
        _cache = cache;
        SeedPredefinedSegments();
    }

    public async Task<SegmentResultDto> EvaluateSegmentAsync(Guid segmentId, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();

        if (!_segments.TryGetValue(segmentId, out var definition))
        {
            _logger.LogWarning("Segment {SegmentId} not found", segmentId);
            sw.Stop();
            return new SegmentResultDto(segmentId, "Unknown", new List<string>(), 0, DateTime.UtcNow, sw.ElapsedMilliseconds, null, null);
        }

        var cacheKey = CacheKeys.SegmentResultKey(segmentId);
        var cached = await _cache.GetAsync<SegmentResultDto>(cacheKey, ct);
        if (cached is not null)
        {
            sw.Stop();
            return cached;
        }

        var result = await EvaluateSegmentDefinitionInternalAsync(definition, ct);
        await _cache.SetAsync(cacheKey, result, DefaultCacheTtl, ct);

        sw.Stop();
        return result;
    }

    public async Task<SegmentResultDto> EvaluateSegmentDefinitionAsync(SegmentDefinitionDto definition, CancellationToken ct = default)
    {
        return await EvaluateSegmentDefinitionInternalAsync(definition, ct);
    }

    public async Task<SegmentPreviewResult> PreviewAsync(SegmentPreviewRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var warnings = new List<string>();

        var sampleUsers = new List<string>();
        var breakdownByRole = new Dictionary<string, int>();

        if (request.Filters is null || request.Filters.Count == 0)
        {
            warnings.Add("No filters provided; returning all users sample.");
            for (int i = 1; i <= 20; i++)
                sampleUsers.Add($"user-{i:D4}");
            breakdownByRole["athlete"] = 10;
            breakdownByRole["coach"] = 5;
            breakdownByRole["parent"] = 3;
            breakdownByRole["academy"] = 2;
        }
        else
        {
            var matchedUsers = EvaluateFiltersOnMockData(request.Filters, request.MatchType);
            sampleUsers = matchedUsers.Take(20).ToList();
            breakdownByRole = EstimateBreakdown(sampleUsers);
        }

        var estimatedCount = Math.Max(sampleUsers.Count * 5, 10);

        sw.Stop();

        return new SegmentPreviewResult(estimatedCount, sampleUsers, breakdownByRole, sw.ElapsedMilliseconds, warnings);
    }

    public Task<SegmentDefinitionDto> CreateAsync(SegmentRequest request, CancellationToken ct = default)
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var definition = new SegmentDefinitionDto(
            id,
            request.Name,
            request.Description,
            request.Type,
            request.MatchType,
            request.Filters ?? new List<SegmentFilterDto>(),
            request.IsDynamic,
            false,
            0,
            null,
            now,
            now
        );

        _segments[id] = definition;
        _logger.LogInformation("Created segment {SegmentId} {Name}", id, request.Name);
        return Task.FromResult(definition);
    }

    public Task<SegmentDefinitionDto> UpdateAsync(Guid id, SegmentRequest request, CancellationToken ct = default)
    {
        if (!_segments.TryGetValue(id, out var existing))
        {
            _logger.LogWarning("Segment {SegmentId} not found for update", id);
            throw new KeyNotFoundException($"Segment with id {id} not found.");
        }

        var updated = new SegmentDefinitionDto(
            id,
            request.Name,
            request.Description,
            request.Type,
            request.MatchType,
            request.Filters ?? new List<SegmentFilterDto>(),
            request.IsDynamic,
            existing.IsSaved,
            existing.EstimatedCount,
            existing.LastCalculatedAt,
            existing.CreatedAt,
            DateTime.UtcNow
        );

        _segments[id] = updated;
        _logger.LogInformation("Updated segment {SegmentId} {Name}", id, request.Name);
        return Task.FromResult(updated);
    }

    public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        if (_segments.TryRemove(id, out _))
        {
            _savedSegments.TryRemove(id, out _);
            _logger.LogInformation("Deleted segment {SegmentId}", id);
            return Task.FromResult(true);
        }

        return Task.FromResult(false);
    }

    public Task<SegmentDefinitionDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        if (_segments.TryGetValue(id, out var definition))
            return Task.FromResult(definition);

        _logger.LogWarning("Segment {SegmentId} not found", id);
        throw new KeyNotFoundException($"Segment with id {id} not found.");
    }

    public Task<SegmentSearchResult> SearchAsync(SegmentSearchCriteria criteria, CancellationToken ct = default)
    {
        var query = _segments.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            var q = criteria.Query.ToLowerInvariant();
            query = query.Where(s => s.Name.ToLowerInvariant().Contains(q) ||
                                     (s.Description?.ToLowerInvariant().Contains(q) ?? false));
        }

        if (criteria.Type.HasValue)
            query = query.Where(s => s.Type == criteria.Type.Value);

        if (criteria.IsSaved.HasValue)
            query = query.Where(s => s.IsSaved == criteria.IsSaved.Value);

        if (criteria.CreatedAfter.HasValue)
            query = query.Where(s => s.CreatedAt >= criteria.CreatedAfter.Value);

        if (criteria.CreatedBefore.HasValue)
            query = query.Where(s => s.CreatedAt <= criteria.CreatedBefore.Value);

        var items = query
            .OrderByDescending(s => s.UpdatedAt)
            .Skip((criteria.PageNumber - 1) * criteria.PageSize)
            .Take(criteria.PageSize)
            .ToList();

        var totalCount = query.Count();
        var hasNextPage = (criteria.PageNumber * criteria.PageSize) < totalCount;

        return Task.FromResult(new SegmentSearchResult(items, totalCount, criteria.PageNumber, criteria.PageSize, hasNextPage));
    }

    public async Task<SegmentResultDto> ResolveSegmentAsync(SegmentType type, Dictionary<string, object>? parameters, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();

        var (userIds, breakdown) = type switch
        {
            SegmentType.Athletes => (GenerateMockIds("athlete", 100), new Dictionary<string, int> { ["athlete"] = 100 }),
            SegmentType.Coaches => (GenerateMockIds("coach", 20), new Dictionary<string, int> { ["coach"] = 20 }),
            SegmentType.Academies => (GenerateMockIds("academy-org", 10), new Dictionary<string, int> { ["academy"] = 10 }),
            SegmentType.Parents => (GenerateMockIds("parent", 50), new Dictionary<string, int> { ["parent"] = 50 }),
            SegmentType.EventParticipants => (GenerateMockIds("event-participant", 30), new Dictionary<string, int> { ["event-participant"] = 30 }),
            SegmentType.TournamentParticipants => (GenerateMockIds("tournament-participant", 50), new Dictionary<string, int> { ["tournament-participant"] = 50 }),
            SegmentType.FinanceDueUsers => (GenerateMockIds("finance-due", 15), new Dictionary<string, int> { ["finance-due"] = 15 }),
            SegmentType.PremiumUsers => (GenerateMockIds("premium", 25), new Dictionary<string, int> { ["premium"] = 25 }),
            SegmentType.NewUsers => (GenerateMockIds("new-user", 10), new Dictionary<string, int> { ["new-user"] = 10 }),
            SegmentType.InactiveUsers => (GenerateMockIds("inactive", 20), new Dictionary<string, int> { ["inactive"] = 20 }),
            SegmentType.AllUsers => (GenerateMockIds("user", 200), new Dictionary<string, int>
            {
                ["athlete"] = 100, ["coach"] = 20, ["parent"] = 50, ["academy"] = 10, ["other"] = 20
            }),
            _ => (new List<string>(), new Dictionary<string, int>())
        };

        sw.Stop();
        return await Task.FromResult(new SegmentResultDto(
            Guid.Empty, type.ToString(), userIds, userIds.Count, DateTime.UtcNow, sw.ElapsedMilliseconds, breakdown, null
        ));
    }

    public async Task<SegmentResultDto> GetAthletesAsync(Dictionary<string, object>? filters, CancellationToken ct = default)
    {
        return await GetPredefinedSegmentAsync("athlete", 100, "athlete", filters, ct);
    }

    public async Task<SegmentResultDto> GetCoachesAsync(Dictionary<string, object>? filters, CancellationToken ct = default)
    {
        return await GetPredefinedSegmentAsync("coach", 20, "coach", filters, ct);
    }

    public async Task<SegmentResultDto> GetAcademiesAsync(Dictionary<string, object>? filters, CancellationToken ct = default)
    {
        return await GetPredefinedSegmentAsync("academy-org", 10, "academy", filters, ct);
    }

    public async Task<SegmentResultDto> GetParentsAsync(Dictionary<string, object>? filters, CancellationToken ct = default)
    {
        return await GetPredefinedSegmentAsync("parent", 50, "parent", filters, ct);
    }

    public async Task<SegmentResultDto> GetEventParticipantsAsync(Guid eventId, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var shortId = eventId.ToString("N")[..8];
        var prefix = $"evt-{shortId}-participant";
        var userIds = GenerateMockIds(prefix, 30);
        var breakdown = new Dictionary<string, int> { ["event-participant"] = 30 };
        sw.Stop();

        return await Task.FromResult(new SegmentResultDto(
            eventId, $"Event Participants ({shortId})", userIds, userIds.Count, DateTime.UtcNow, sw.ElapsedMilliseconds, breakdown, null
        ));
    }

    public async Task<SegmentResultDto> GetTournamentParticipantsAsync(Guid tournamentId, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var shortId = tournamentId.ToString("N")[..8];
        var prefix = $"trn-{shortId}-participant";
        var userIds = GenerateMockIds(prefix, 50);
        var breakdown = new Dictionary<string, int> { ["tournament-participant"] = 50 };
        sw.Stop();

        return await Task.FromResult(new SegmentResultDto(
            tournamentId, $"Tournament Participants ({shortId})", userIds, userIds.Count, DateTime.UtcNow, sw.ElapsedMilliseconds, breakdown, null
        ));
    }

    public async Task<SegmentResultDto> GetFinanceDueUsersAsync(decimal? minAmount, DateTime? dueBefore, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var userIds = GenerateMockIds("finance-due", 15);
        var breakdown = new Dictionary<string, int> { ["finance-due"] = 15 };
        sw.Stop();

        return await Task.FromResult(new SegmentResultDto(
            Guid.Empty, "Finance Due Users", userIds, userIds.Count, DateTime.UtcNow, sw.ElapsedMilliseconds, breakdown,
            new Dictionary<string, int> { ["overdue"] = 15 }
        ));
    }

    public async Task<SegmentResultDto> GetInactiveUsersAsync(TimeSpan inactivityPeriod, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var userIds = GenerateMockIds("inactive", 20);
        var breakdown = new Dictionary<string, int> { ["inactive"] = 20 };
        sw.Stop();

        return await Task.FromResult(new SegmentResultDto(
            Guid.Empty, $"Inactive Users ({inactivityPeriod.TotalDays}d+)", userIds, userIds.Count, DateTime.UtcNow, sw.ElapsedMilliseconds, breakdown, null
        ));
    }

    public async Task<SegmentResultDto> GetPremiumUsersAsync(CancellationToken ct = default)
    {
        return await GetPredefinedSegmentAsync("premium", 25, "premium", null, ct);
    }

    public async Task<SegmentResultDto> GetNewUsersAsync(DateTime since, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var userIds = GenerateMockIds("new-user", 10);
        var breakdown = new Dictionary<string, int> { ["new-user"] = 10 };
        sw.Stop();

        return await Task.FromResult(new SegmentResultDto(
            Guid.Empty, $"New Users (since {since:yyyy-MM-dd})", userIds, userIds.Count, DateTime.UtcNow, sw.ElapsedMilliseconds, breakdown, null
        ));
    }

    public Task<List<SavedSegmentDto>> GetSavedSegmentsAsync(CancellationToken ct = default)
    {
        var result = _savedSegments.Values
            .OrderByDescending(s => s.LastUsedAt)
            .ToList();
        return Task.FromResult(result);
    }

    public Task<SegmentDefinitionDto> SaveSegmentAsync(Guid segmentId, CancellationToken ct = default)
    {
        if (!_segments.TryGetValue(segmentId, out var definition))
        {
            _logger.LogWarning("Segment {SegmentId} not found for save", segmentId);
            throw new KeyNotFoundException($"Segment with id {segmentId} not found.");
        }

        var saved = new SavedSegmentDto(
            definition.Id,
            definition.Name,
            definition.Description,
            definition.Type,
            definition.EstimatedCount,
            DateTime.UtcNow,
            1,
            definition.CreatedAt
        );

        _savedSegments[segmentId] = saved;

        var updated = new SegmentDefinitionDto(
            definition.Id,
            definition.Name,
            definition.Description,
            definition.Type,
            definition.MatchType,
            definition.Filters,
            definition.IsDynamic,
            true,
            definition.EstimatedCount,
            definition.LastCalculatedAt,
            definition.CreatedAt,
            DateTime.UtcNow
        );

        _segments[segmentId] = updated;
        _logger.LogInformation("Saved segment {SegmentId} {Name}", segmentId, definition.Name);
        return Task.FromResult(updated);
    }

    public Task<List<RuleDefinitionDto>> GetAvailableRulesAsync(CancellationToken ct = default)
    {
        var rules = new List<RuleDefinitionDto>
        {
            new("Role", "User role (athlete, coach, parent, academy)", "user.role", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in" }, null),
            new("Tags", "User tags/labels", "user.tags", "contains", "string",
                new List<string> { "contains", "not_equals", "exists", "not_exists" }, null),
            new("Age", "User age in years", "user.profile.age", "greater_than", "integer",
                new List<string> { "equals", "not_equals", "greater_than", "less_than", "between" }, @"^\d+$"),
            new("Gender", "User gender", "user.profile.gender", "equals", "string",
                new List<string> { "equals", "not_equals" }, null),
            new("Location City", "User city", "user.profile.address.city", "equals", "string",
                new List<string> { "equals", "not_equals", "contains", "in", "not_in" }, null),
            new("Location State", "User state/province", "user.profile.address.state", "equals", "string",
                new List<string> { "equals", "not_equals", "contains", "in", "not_in" }, null),
            new("Location Country", "User country", "user.profile.address.country", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in" }, null),
            new("Sport", "Primary sport", "user.profile.sport", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in", "contains" }, null),
            new("Skill Level", "Skill level (beginner, intermediate, advanced)", "user.profile.skillLevel", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in" }, null),
            new("Experience Years", "Years of experience", "user.profile.experienceYears", "greater_than", "integer",
                new List<string> { "equals", "not_equals", "greater_than", "less_than", "between" }, @"^\d+$"),
            new("Registration Date", "Date of registration", "user.createdAt", "between", "datetime",
                new List<string> { "equals", "between", "greater_than", "less_than" }, null),
            new("Last Login", "Last login date", "user.lastLoginAt", "between", "datetime",
                new List<string> { "between", "greater_than", "less_than", "exists", "not_exists" }, null),
            new("Academy Name", "Name of the academy", "user.academy.name", "contains", "string",
                new List<string> { "equals", "not_equals", "contains", "exists", "not_exists" }, null),
            new("Academy Type", "Type of academy", "user.academy.type", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in" }, null),
            new("Subscription Plan", "Subscription plan name", "user.subscription.plan", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in", "exists", "not_exists" }, null),
            new("Subscription Status", "Subscription status (active, expired, cancelled)", "user.subscription.status", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in" }, null),
            new("Payment Overdue", "Has overdue payments", "user.finance.hasOverdue", "equals", "boolean",
                new List<string> { "equals", "exists" }, null),
            new("Outstanding Amount", "Outstanding payment amount", "user.finance.outstandingAmount", "greater_than", "decimal",
                new List<string> { "equals", "greater_than", "less_than", "between" }, @"^\d+(\.\d+)?$"),
            new("Event Participation Count", "Number of events participated", "user.stats.eventCount", "greater_than", "integer",
                new List<string> { "equals", "greater_than", "less_than", "between" }, @"^\d+$"),
            new("Tournament Participation Count", "Number of tournaments participated", "user.stats.tournamentCount", "greater_than", "integer",
                new List<string> { "equals", "greater_than", "less_than", "between" }, @"^\d+$"),
            new("HasPhone", "User has verified phone", "user.contact.hasPhone", "equals", "boolean",
                new List<string> { "equals", "exists" }, null),
            new("HasEmail", "User has verified email", "user.contact.hasEmail", "equals", "boolean",
                new List<string> { "equals", "exists" }, null),
            new("Age Group", "Age group category", "user.profile.ageGroup", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in" }, null),
            new("Membership Tier", "Membership tier level", "user.membership.tier", "equals", "string",
                new List<string> { "equals", "not_equals", "in", "not_in", "exists", "not_exists" }, null),
        };

        return Task.FromResult(rules);
    }

    private async Task<SegmentResultDto> EvaluateSegmentDefinitionInternalAsync(SegmentDefinitionDto definition, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();

        if (definition.Type is SegmentType.CustomDynamic or SegmentType.AllUsers or SegmentType.ByRole or SegmentType.ByTag)
        {
            var userIds = EvaluateFiltersOnMockData(definition.Filters, definition.MatchType);
            var breakdown = EstimateBreakdown(userIds);
            sw.Stop();

            var result = new SegmentResultDto(
                definition.Id,
                definition.Name,
                userIds,
                userIds.Count,
                DateTime.UtcNow,
                sw.ElapsedMilliseconds,
                breakdown,
                null
            );

            return await Task.FromResult(result);
        }

        var resolved = await ResolveSegmentAsync(definition.Type, null, ct);
        var filteredUserIds = definition.Filters is not null && definition.Filters.Count > 0
            ? EvaluateFiltersOnMockData(definition.Filters, definition.MatchType)
            : resolved.UserIds;

        sw.Stop();
        return new SegmentResultDto(
            definition.Id,
            definition.Name,
            filteredUserIds,
            filteredUserIds.Count,
            DateTime.UtcNow,
            sw.ElapsedMilliseconds,
            resolved.BreakdownByRole,
            resolved.BreakdownByTag
        );
    }

    private async Task<SegmentResultDto> GetPredefinedSegmentAsync(
        string prefix, int count, string role, Dictionary<string, object>? filters, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        var userIds = GenerateMockIds(prefix, count);

        if (filters is not null && filters.Count > 0)
        {
            var filterList = filters.Select(f => new SegmentFilterDto(f.Key, "equals", f.Value, null, null)).ToList();
            var matchType = SegmentMatchType.All;
            userIds = EvaluateFiltersOnMockData(filterList, matchType);
        }

        var breakdown = new Dictionary<string, int> { [role] = userIds.Count };
        sw.Stop();

        return await Task.FromResult(new SegmentResultDto(
            Guid.Empty, $"{char.ToUpper(prefix[0]) + prefix[1..]} Users", userIds, userIds.Count, DateTime.UtcNow, sw.ElapsedMilliseconds, breakdown, null
        ));
    }

    private List<string> EvaluateFiltersOnMockData(List<SegmentFilterDto> filters, SegmentMatchType matchType)
    {
        var allMockUsers = GenerateAllMockUsers();

        if (filters is null || filters.Count == 0)
            return allMockUsers;

        return matchType switch
        {
            SegmentMatchType.All => allMockUsers.Where(u => filters.All(f => EvaluateFilter(u, f))).ToList(),
            SegmentMatchType.Any => allMockUsers.Where(u => filters.Any(f => EvaluateFilter(u, f))).ToList(),
            SegmentMatchType.None => allMockUsers.Where(u => !filters.Any(f => EvaluateFilter(u, f))).ToList(),
            _ => allMockUsers.Where(u => filters.All(f => EvaluateFilter(u, f))).ToList()
        };
    }

    private bool EvaluateFilter(string userId, SegmentFilterDto filter)
    {
        try
        {
            var fieldValue = ResolveFieldValue(userId, filter.Field);
            var filterValue = filter.Value?.ToString();
            var secondaryValue = filter.SecondaryValue?.ToString();

            return filter.Operator?.ToLowerInvariant() switch
            {
                "equals" => string.Equals(fieldValue, filterValue, StringComparison.OrdinalIgnoreCase),
                "not_equals" => !string.Equals(fieldValue, filterValue, StringComparison.OrdinalIgnoreCase),
                "contains" => fieldValue?.Contains(filterValue ?? string.Empty, StringComparison.OrdinalIgnoreCase) ?? false,
                "greater_than" => CompareValues(fieldValue, filterValue) > 0,
                "less_than" => CompareValues(fieldValue, filterValue) < 0,
                "between" => IsBetween(fieldValue, filterValue, secondaryValue),
                "in" => (filterValue?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>())
                    .Contains(fieldValue, StringComparer.OrdinalIgnoreCase),
                "not_in" => !(filterValue?.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) ?? Array.Empty<string>())
                    .Contains(fieldValue, StringComparer.OrdinalIgnoreCase),
                "exists" => fieldValue is not null,
                "not_exists" => fieldValue is null,
                _ => true
            };
        }
        catch
        {
            return true;
        }
    }

    private static string? ResolveFieldValue(string userId, string fieldPath)
    {
        if (string.IsNullOrWhiteSpace(fieldPath))
            return userId;

        var parts = fieldPath.Split('.', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length == 0)
            return userId;

        if (parts[0] == "user")
        {
            if (parts.Length == 1) return userId;

            return parts[1] switch
            {
                "id" => userId,
                "role" => userId.Contains("athlete") ? "athlete" :
                          userId.Contains("coach") ? "coach" :
                          userId.Contains("parent") ? "parent" :
                          userId.Contains("academy") ? "academy" :
                          userId.Contains("premium") ? "premium" :
                          userId.Contains("inactive") ? "inactive" : "unknown",
                "tags" when parts.Length > 2 => parts[2],
                "tags" => $"tag-{Math.Abs(userId.GetHashCode()) % 5}",
                "createdAt" => DateTime.UtcNow.AddDays(-Math.Abs(userId.GetHashCode()) % 365).ToString("O"),
                "lastLoginAt" => DateTime.UtcNow.AddDays(-Math.Abs(userId.GetHashCode()) % 90).ToString("O"),
                _ => ResolveNestedField(userId, parts)
            };
        }

        return userId;
    }

    private static string? ResolveNestedField(string userId, string[] parts)
    {
        var subKey = parts.Length > 1 ? parts[1] : parts[0];

        return subKey switch
        {
            "profile" when parts.Length > 2 => parts[2] switch
            {
                "age" => (18 + Math.Abs(userId.GetHashCode()) % 40).ToString(),
                "gender" => Math.Abs(userId.GetHashCode()) % 2 == 0 ? "male" : "female",
                "sport" => new[] { "cricket", "football", "tennis", "badminton", "swimming", "athletics" }[Math.Abs(userId.GetHashCode()) % 6],
                "skillLevel" => new[] { "beginner", "intermediate", "advanced" }[Math.Abs(userId.GetHashCode()) % 3],
                "experienceYears" => (1 + Math.Abs(userId.GetHashCode()) % 20).ToString(),
                "ageGroup" => new[] { "U12", "U14", "U16", "U18", "Senior" }[Math.Abs(userId.GetHashCode()) % 5],
                _ => null
            },
            "address" when parts.Length > 3 => parts[3] switch
            {
                "city" => new[] { "Mumbai", "Delhi", "Bangalore", "Chennai", "Kolkata", "Pune", "Hyderabad" }[Math.Abs(userId.GetHashCode()) % 7],
                "state" => new[] { "Maharashtra", "Delhi", "Karnataka", "Tamil Nadu", "West Bengal" }[Math.Abs(userId.GetHashCode()) % 5],
                "country" => "India",
                _ => null
            },
            "academy" when parts.Length > 2 => parts[2] switch
            {
                "name" => new[] { "Elite Sports Academy", "Champions Academy", "Pro Training Center", "Ace Sports Institute", "Legends Academy" }[Math.Abs(userId.GetHashCode()) % 5],
                "type" => new[] { "multi-sport", "single-sport", "specialized" }[Math.Abs(userId.GetHashCode()) % 3],
                _ => null
            },
            "subscription" when parts.Length > 2 => parts[2] switch
            {
                "plan" => new[] { "basic", "standard", "premium", "enterprise" }[Math.Abs(userId.GetHashCode()) % 4],
                "status" => new[] { "active", "expired", "cancelled" }[Math.Abs(userId.GetHashCode()) % 3],
                _ => null
            },
            "finance" when parts.Length > 2 => parts[2] switch
            {
                "hasOverdue" => (Math.Abs(userId.GetHashCode()) % 10 < 3).ToString().ToLower(),
                "outstandingAmount" => (1000 + Math.Abs(userId.GetHashCode()) % 50000).ToString("F2"),
                _ => null
            },
            "stats" when parts.Length > 2 => parts[2] switch
            {
                "eventCount" => (Math.Abs(userId.GetHashCode()) % 30).ToString(),
                "tournamentCount" => (Math.Abs(userId.GetHashCode()) % 15).ToString(),
                _ => null
            },
            "contact" when parts.Length > 2 => parts[2] switch
            {
                "hasPhone" => (Math.Abs(userId.GetHashCode()) % 10 < 8).ToString().ToLower(),
                "hasEmail" => (Math.Abs(userId.GetHashCode()) % 10 < 9).ToString().ToLower(),
                _ => null
            },
            "membership" when parts.Length > 2 => parts[2] switch
            {
                "tier" => new[] { "bronze", "silver", "gold", "platinum" }[Math.Abs(userId.GetHashCode()) % 4],
                _ => null
            },
            _ => null
        };
    }

    private static int CompareValues(string? a, string? b)
    {
        if (a is null && b is null) return 0;
        if (a is null) return -1;
        if (b is null) return 1;

        if (decimal.TryParse(a, out var decA) && decimal.TryParse(b, out var decB))
            return decA.CompareTo(decB);

        if (DateTime.TryParse(a, out var dtA) && DateTime.TryParse(b, out var dtB))
            return dtA.CompareTo(dtB);

        return string.Compare(a, b, StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsBetween(string? value, string? lower, string? upper)
    {
        if (value is null || lower is null) return false;
        return CompareValues(value, lower) >= 0 && (upper is null || CompareValues(value, upper) <= 0);
    }

    private List<string> GenerateAllMockUsers()
    {
        var users = new List<string>();
        users.AddRange(GenerateMockIds("athlete", 100));
        users.AddRange(GenerateMockIds("coach", 20));
        users.AddRange(GenerateMockIds("parent", 50));
        users.AddRange(GenerateMockIds("academy-org", 10));
        users.AddRange(GenerateMockIds("premium", 25));
        users.AddRange(GenerateMockIds("inactive", 20));
        return users;
    }

    private static List<string> GenerateMockIds(string prefix, int count)
    {
        var ids = new List<string>(count);
        for (int i = 1; i <= count; i++)
            ids.Add($"{prefix}-{i:D4}");
        return ids;
    }

    private static Dictionary<string, int> EstimateBreakdown(List<string> userIds)
    {
        var breakdown = new Dictionary<string, int>();
        foreach (var uid in userIds)
        {
            var role = uid.Contains("athlete") ? "athlete" :
                       uid.Contains("coach") ? "coach" :
                       uid.Contains("parent") ? "parent" :
                       uid.Contains("academy") ? "academy" :
                       uid.Contains("premium") ? "premium" : "other";
            breakdown.TryGetValue(role, out var count);
            breakdown[role] = count + 1;
        }
        return breakdown;
    }

    private void SeedPredefinedSegments()
    {
        var now = DateTime.UtcNow;
        var predefined = new (string name, SegmentType type, string prefix, int count)[]
        {
            ("All Athletes", SegmentType.Athletes, "athlete", 100),
            ("All Coaches", SegmentType.Coaches, "coach", 20),
            ("All Academies", SegmentType.Academies, "academy-org", 10),
            ("All Parents", SegmentType.Parents, "parent", 50),
            ("Premium Users", SegmentType.PremiumUsers, "premium", 25),
            ("Inactive Users", SegmentType.InactiveUsers, "inactive", 20),
        };

        foreach (var (name, type, prefix, count) in predefined)
        {
            var id = Guid.NewGuid();
            _segments[id] = new SegmentDefinitionDto(
                id, name, $"Predefined segment for {name}", type, SegmentMatchType.All,
                new List<SegmentFilterDto>(), false, false, count, now, now, now
            );
        }

        foreach (var (name, type, prefix, count) in predefined)
        {
            var ids = GenerateMockIds(prefix, count);
            _predefinedSegments[type.ToString()] = ids;
        }
    }
}
