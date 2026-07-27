namespace SportsGurukul.Application.Features.SharedScheduling.Models;

public enum RecurrenceFrequency { Daily = 0, Weekly = 1, Monthly = 2, Yearly = 3, Custom = 4 }

public sealed record RecurrencePattern
{
    public RecurrenceFrequency Frequency { get; init; }
    public int Interval { get; init; } = 1;
    public int? MaxOccurrences { get; init; }
    public DateTime? EndDate { get; init; }
    public IReadOnlyList<DayOfWeek> DaysOfWeek { get; init; } = [];
    public int? DayOfMonth { get; init; }
    public int? MonthOfYear { get; init; }
    public string? RRule { get; init; }
    public IReadOnlyList<DateTime> ExceptionDates { get; init; } = [];
    public bool SkipHolidays { get; init; } = true;
}
