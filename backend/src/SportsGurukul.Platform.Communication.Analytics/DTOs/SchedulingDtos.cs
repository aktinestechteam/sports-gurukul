namespace SportsGurukul.Platform.Communication.Analytics.DTOs;

public record CronExpressionDto(
    string Expression,
    string? Description,
    DateTime? NextOccurrence,
    List<DateTime>? UpcomingOccurrences,
    bool IsValid,
    string? ValidationError
);

public record TimeZoneInfoDto(
    string Id,
    string DisplayName,
    TimeSpan UtcOffset,
    bool SupportsDaylightSaving
);

public record BusinessHoursDto(
    Guid Id,
    string Name,
    Dictionary<DayOfWeek, TimeRangeDto> WeekHours,
    List<DateOverrideDto>? Overrides,
    string? TimeZone,
    bool IsEnabled
);

public record TimeRangeDto(
    TimeSpan Start,
    TimeSpan End,
    bool IsActive
);

public record DateOverrideDto(
    DateTime Date,
    TimeRangeDto? OverrideHours,
    bool IsClosed,
    string? Reason
);

public record QuietHoursDto(
    Guid Id,
    string Name,
    TimeSpan Start,
    TimeSpan End,
    string? TimeZone,
    List<DayOfWeek>? ApplicableDays,
    bool IsEnabled,
    bool AllowUrgent
);

public record HolidayCalendarDto(
    Guid Id,
    string Name,
    string? Description,
    string? Country,
    int Year,
    List<HolidayDateDto> Holidays,
    bool IsEnabled
);

public record HolidayDateDto(
    DateTime Date,
    string Name,
    bool IsRecurring,
    string? Category
);

public record ScheduleValidationResult(
    bool IsValid,
    List<string> Warnings,
    List<string> Errors,
    DateTime? NextValidOccurrence,
    List<DateTime>? UpcomingOccurrences,
    long EstimatedExecutionTimeMs,
    bool ConflictsWithQuietHours,
    bool ConflictsWithHoliday
);

public record RetryWindowDto(
    Guid Id,
    string Name,
    int MaxRetries,
    TimeSpan InitialDelay,
    TimeSpan MaxDelay,
    double BackoffMultiplier,
    RetryBackoffStrategy BackoffStrategy,
    List<TimeSpan>? FixedDelays,
    bool ExponentialBackoff
);

public enum RetryBackoffStrategy
{
    Fixed,
    Linear,
    Exponential,
    Fibonacci,
    Custom
}

public record ScheduleJobDto(
    Guid Id,
    string JobType,
    string? JobData,
    ScheduleDefinitionDto Schedule,
    bool IsActive,
    DateTime? LastRunAt,
    DateTime? NextRunAt,
    int TotalRuns,
    int SuccessfulRuns,
    int FailedRuns,
    DateTime CreatedAt
);

public record ScheduleExecutionResult(
    Guid JobId,
    bool Success,
    DateTime ExecutedAt,
    DateTime? NextExecutionAt,
    long DurationMs,
    string? Error,
    int AttemptNumber
);
