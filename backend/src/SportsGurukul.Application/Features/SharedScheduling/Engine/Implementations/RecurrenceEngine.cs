using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public partial class RecurrenceEngine : IRecurrenceEngine
{
    private readonly ILogger<RecurrenceEngine> _logger;

    public RecurrenceEngine(ILogger<RecurrenceEngine> logger) => _logger = logger;

    public IReadOnlyList<DateTime> GenerateOccurrences(RecurrencePattern pattern, DateTime startDate)
    {
        var dates = new List<DateTime>();
        var current = startDate;
        var count = 0;
        var maxOccurrences = pattern.MaxOccurrences ?? 365;
        var finalDate = pattern.EndDate ?? startDate.AddYears(5);

        while (count < maxOccurrences && current.Date <= finalDate.Date)
        {
            if (!pattern.ExceptionDates.Contains(current.Date))
            {
                dates.Add(current);
            }

            count++;
            current = pattern.Frequency switch
            {
                RecurrenceFrequency.Daily => current.AddDays(pattern.Interval),
                RecurrenceFrequency.Weekly => AdvanceWeekly(current, pattern),
                RecurrenceFrequency.Monthly => AdvanceMonthly(current, pattern),
                RecurrenceFrequency.Yearly => current.AddYears(pattern.Interval),
                RecurrenceFrequency.Custom => !string.IsNullOrWhiteSpace(pattern.RRule)
                    ? AdvanceCustomRRule(current, pattern.RRule)
                    : current.AddDays(pattern.Interval),
                _ => current.AddDays(1)
            };
        }

        _logger.LogDebug("Generated {Count} occurrences for frequency {Frequency}", dates.Count, pattern.Frequency);
        return dates;
    }

    public IReadOnlyList<DateTime> FilterOccurrences(IReadOnlyList<DateTime> occurrences, SchedulingContext context)
    {
        var holidays = new HashSet<DateTime>(context.Holidays.Select(h => h.Date.Date));
        return occurrences.Where(d =>
            (!context.Holidays.Any() || !holidays.Contains(d.Date)) ||
            context.Holidays.All(h => h.Date.Date != d.Date || !h.IsRecurring)
        ).ToList();
    }

    public IReadOnlyList<DateTime> ParseRRule(string rRule, DateTime startDate, int maxOccurrences = 365)
    {
        var dates = new List<DateTime>();
        var current = startDate;

        var freqMatch = RRuleFreqRegex().Match(rRule);
        var intervalMatch = RRuleIntervalRegex().Match(rRule);
        var countMatch = RRuleCountRegex().Match(rRule);
        var untilMatch = RRuleUntilRegex().Match(rRule);

        var freq = freqMatch.Success ? freqMatch.Groups[1].Value : "DAILY";
        var interval = intervalMatch.Success ? int.Parse(intervalMatch.Groups[1].Value) : 1;
        var maxCount = countMatch.Success ? Math.Min(int.Parse(countMatch.Groups[1].Value), maxOccurrences) : maxOccurrences;
        var until = untilMatch.Success && DateTime.TryParseExact(untilMatch.Groups[1].Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var u) ? u : startDate.AddYears(5);

        for (int i = 0; i < maxCount && current.Date <= until.Date; i++)
        {
            dates.Add(current);
            current = freq switch
            {
                "DAILY" => current.AddDays(interval),
                "WEEKLY" => AdvanceWeeklyFromRRule(current, rRule, interval),
                "MONTHLY" => current.AddMonths(interval),
                "YEARLY" => current.AddYears(interval),
                _ => current.AddDays(interval)
            };
        }

        return dates;
    }

    public RecurrencePattern? TryParseRRule(string rRule)
    {
        if (string.IsNullOrWhiteSpace(rRule)) return null;

        var freqMatch = RRuleFreqRegex().Match(rRule);
        if (!freqMatch.Success) return null;

        var intervalMatch = RRuleIntervalRegex().Match(rRule);
        var countMatch = RRuleCountRegex().Match(rRule);
        var untilMatch = RRuleUntilRegex().Match(rRule);
        var byDayMatch = RRuleByDayRegex().Match(rRule);

        var frequency = freqMatch.Groups[1].Value switch
        {
            "DAILY" => RecurrenceFrequency.Daily,
            "WEEKLY" => RecurrenceFrequency.Weekly,
            "MONTHLY" => RecurrenceFrequency.Monthly,
            "YEARLY" => RecurrenceFrequency.Yearly,
            _ => RecurrenceFrequency.Custom
        };

        var daysOfWeek = new List<DayOfWeek>();
        if (byDayMatch.Success)
        {
            foreach (var day in byDayMatch.Groups[1].Value.Split(','))
            {
                if (DayAbbrevMap().TryGetValue(day.Trim(), out var dow))
                    daysOfWeek.Add(dow);
            }
        }

        return new RecurrencePattern
        {
            Frequency = frequency,
            Interval = intervalMatch.Success ? int.Parse(intervalMatch.Groups[1].Value) : 1,
            MaxOccurrences = countMatch.Success ? int.Parse(countMatch.Groups[1].Value) : null,
            EndDate = untilMatch.Success && DateTime.TryParseExact(untilMatch.Groups[1].Value, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var u) ? u : null,
            DaysOfWeek = daysOfWeek,
            RRule = rRule
        };
    }

    public string ToRRule(RecurrencePattern pattern)
    {
        var parts = new List<string> { $"FREQ={pattern.Frequency.ToString().ToUpperInvariant()}" };
        if (pattern.Interval > 1) parts.Add($"INTERVAL={pattern.Interval}");
        if (pattern.MaxOccurrences.HasValue) parts.Add($"COUNT={pattern.MaxOccurrences.Value}");
        if (pattern.EndDate.HasValue) parts.Add($"UNTIL={pattern.EndDate.Value:yyyyMMdd}T235959Z");
        if (pattern.DaysOfWeek.Count > 0) parts.Add($"BYDAY={string.Join(",", pattern.DaysOfWeek.Select(d => DayToAbbrev(d)))}");
        return string.Join(";", parts);
    }

    private static DateTime AdvanceWeekly(DateTime current, RecurrencePattern pattern)
    {
        if (pattern.DaysOfWeek.Count > 0)
        {
            for (int i = 1; i <= 7 * pattern.Interval; i++)
            {
                var next = current.AddDays(i);
                if (pattern.DaysOfWeek.Contains(next.DayOfWeek))
                    return next;
            }
        }
        return current.AddDays(7 * pattern.Interval);
    }

    private static DateTime AdvanceMonthly(DateTime current, RecurrencePattern pattern)
    {
        if (pattern.DayOfMonth.HasValue)
        {
            var next = current.AddMonths(pattern.Interval);
            var targetDay = Math.Min(pattern.DayOfMonth.Value, DateTime.DaysInMonth(next.Year, next.Month));
            return new DateTime(next.Year, next.Month, targetDay);
        }
        return current.AddMonths(pattern.Interval);
    }

    private static DateTime AdvanceCustomRRule(DateTime current, string rRule)
    {
        var freqMatch = RRuleFreqRegex().Match(rRule);
        var intervalMatch = RRuleIntervalRegex().Match(rRule);
        var interval = intervalMatch.Success ? int.Parse(intervalMatch.Groups[1].Value) : 1;

        return freqMatch.Groups[1].Value switch
        {
            "DAILY" => current.AddDays(interval),
            "WEEKLY" => current.AddDays(7 * interval),
            "MONTHLY" => current.AddMonths(interval),
            "YEARLY" => current.AddYears(interval),
            _ => current.AddDays(interval)
        };
    }

    private static DateTime AdvanceWeeklyFromRRule(DateTime current, string rRule, int interval)
    {
        var byDayMatch = RRuleByDayRegex().Match(rRule);
        if (byDayMatch.Success)
        {
            var days = byDayMatch.Groups[1].Value.Split(',')
                .Select(d => DayAbbrevMap().GetValueOrDefault(d.Trim()))
                .Where(d => d != default)
                .ToList();

            for (int i = 1; i <= 7 * interval; i++)
            {
                var next = current.AddDays(i);
                if (days.Contains(next.DayOfWeek))
                    return next;
            }
        }
        return current.AddDays(7 * interval);
    }

    private static string DayToAbbrev(DayOfWeek day) => day switch
    {
        DayOfWeek.Monday => "MO", DayOfWeek.Tuesday => "TU", DayOfWeek.Wednesday => "WE",
        DayOfWeek.Thursday => "TH", DayOfWeek.Friday => "FR", DayOfWeek.Saturday => "SA", DayOfWeek.Sunday => "SU",
        _ => "MO"
    };

    [GeneratedRegex("FREQ=(\\w+)")]
    private static partial Regex RRuleFreqRegex();

    [GeneratedRegex("INTERVAL=(\\d+)")]
    private static partial Regex RRuleIntervalRegex();

    [GeneratedRegex("COUNT=(\\d+)")]
    private static partial Regex RRuleCountRegex();

    [GeneratedRegex("UNTIL=(\\d{8})")]
    private static partial Regex RRuleUntilRegex();

    [GeneratedRegex("BYDAY=([\\w,]+)")]
    private static partial Regex RRuleByDayRegex();

    [GeneratedRegex("[A-Z]{2}")]
    private static partial Regex DayAbbrevRegex();

    private static readonly Dictionary<string, DayOfWeek> _dayAbbrevMap = new()
    {
        ["MO"] = DayOfWeek.Monday, ["TU"] = DayOfWeek.Tuesday, ["WE"] = DayOfWeek.Wednesday,
        ["TH"] = DayOfWeek.Thursday, ["FR"] = DayOfWeek.Friday, ["SA"] = DayOfWeek.Saturday, ["SU"] = DayOfWeek.Sunday
    };

    private static Dictionary<string, DayOfWeek> DayAbbrevMap() => _dayAbbrevMap;
}
