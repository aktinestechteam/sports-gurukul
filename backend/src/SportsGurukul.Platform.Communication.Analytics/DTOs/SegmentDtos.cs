namespace SportsGurukul.Platform.Communication.Analytics.DTOs;

public enum SegmentType
{
    AllUsers,
    ByRole,
    ByTag,
    CustomDynamic,
    Saved,
    Athletes,
    Coaches,
    Academies,
    Parents,
    EventParticipants,
    TournamentParticipants,
    FinanceDueUsers,
    InactiveUsers,
    NewUsers,
    PremiumUsers
}

public enum SegmentMatchType
{
    All,
    Any,
    None
}

public record SegmentDefinitionDto(
    Guid Id,
    string Name,
    string? Description,
    SegmentType Type,
    SegmentMatchType MatchType,
    List<SegmentFilterDto> Filters,
    bool IsDynamic,
    bool IsSaved,
    int EstimatedCount,
    DateTime? LastCalculatedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record SegmentFilterDto(
    string Field,
    string Operator,
    object? Value,
    object? SecondaryValue,
    string? DataType
);

public record SegmentResultDto(
    Guid SegmentId,
    string SegmentName,
    List<string> UserIds,
    int TotalCount,
    DateTime CalculatedAt,
    long CalculationTimeMs,
    Dictionary<string, int>? BreakdownByRole,
    Dictionary<string, int>? BreakdownByTag
);

public record RuleDefinitionDto(
    string Name,
    string Description,
    string FieldPath,
    string Operator,
    string ValueType,
    List<string>? AllowedOperators,
    string? ValidationRegex
);

public record SegmentRequest(
    string Name,
    string? Description,
    SegmentType Type,
    SegmentMatchType MatchType,
    List<SegmentFilterDto> Filters,
    bool IsDynamic
);

public record SegmentPreviewRequest(
    List<SegmentFilterDto> Filters,
    SegmentMatchType MatchType
);

public record SegmentPreviewResult(
    int EstimatedCount,
    List<string>? SampleUserIds,
    Dictionary<string, int>? BreakdownByRole,
    long CalculationTimeMs,
    List<string> ValidationWarnings
);

public record SavedSegmentDto(
    Guid Id,
    string Name,
    string? Description,
    SegmentType Type,
    int EstimatedCount,
    DateTime LastUsedAt,
    int UseCount,
    DateTime CreatedAt
);

public record SegmentSearchCriteria(
    string? Query,
    SegmentType? Type,
    bool? IsSaved,
    DateTime? CreatedAfter,
    DateTime? CreatedBefore,
    int PageNumber = 1,
    int PageSize = 20
);

public record SegmentSearchResult(
    List<SegmentDefinitionDto> Items,
    int TotalCount,
    int PageNumber,
    int PageSize,
    bool HasNextPage
);
