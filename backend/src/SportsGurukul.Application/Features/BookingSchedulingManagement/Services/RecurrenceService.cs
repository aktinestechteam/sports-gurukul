using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Services;

public class RecurrenceService : IRecurrenceService
{
    public IReadOnlyList<DateTime> GenerateOccurrences(
        RecurrenceType recurrenceType,
        DateTime startDate,
        TimeSpan startTime,
        TimeSpan endTime,
        int? occurrenceCount,
        DateTime? endDate,
        string? rRule = null,
        string? exceptions = null)
    {
        var dates = new List<DateTime>();
        var current = startDate;
        var count = 0;
        var maxOccurrences = occurrenceCount ?? 365;
        var finalDate = endDate ?? startDate.AddYears(1);

        var exceptionDates = ParseExceptions(exceptions);

        while (count < maxOccurrences && current.Date <= finalDate.Date)
        {
            if (!exceptionDates.Contains(current.Date))
            {
                dates.Add(current);
            }

            count++;
            current = recurrenceType switch
            {
                RecurrenceType.Daily => current.AddDays(1),
                RecurrenceType.Weekly => current.AddDays(7),
                RecurrenceType.Monthly => AddMonths(current),
                RecurrenceType.Custom => ParseCustomRRule(current, rRule),
                _ => current.AddDays(1)
            };
        }

        return dates;
    }

    private static DateTime AddMonths(DateTime date)
    {
        return date.AddMonths(1);
    }

    private static DateTime ParseCustomRRule(DateTime current, string? rRule)
    {
        if (string.IsNullOrWhiteSpace(rRule))
            return current.AddDays(1);

        if (rRule.StartsWith("FREQ=DAILY;INTERVAL=") &&
            int.TryParse(rRule.Split("INTERVAL=")[1].Split(';')[0], out int dailyInterval))
        {
            return current.AddDays(dailyInterval);
        }

        if (rRule.StartsWith("FREQ=WEEKLY;BYDAY="))
        {
            var days = rRule.Split("BYDAY=")[1].Split(',');
            var dayMap = new Dictionary<string, DayOfWeek>
            {
                ["MO"] = DayOfWeek.Monday,
                ["TU"] = DayOfWeek.Tuesday,
                ["WE"] = DayOfWeek.Wednesday,
                ["TH"] = DayOfWeek.Thursday,
                ["FR"] = DayOfWeek.Friday,
                ["SA"] = DayOfWeek.Saturday,
                ["SU"] = DayOfWeek.Sunday
            };

            for (int i = 1; i <= 7; i++)
            {
                var nextDate = current.AddDays(i);
                if (days.Any(d => dayMap.TryGetValue(d, out var dow) && nextDate.DayOfWeek == dow))
                    return nextDate;
            }
        }

        return current.AddDays(7);
    }

    private static HashSet<DateTime> ParseExceptions(string? exceptions)
    {
        var dates = new HashSet<DateTime>();
        if (string.IsNullOrWhiteSpace(exceptions))
            return dates;

        foreach (var part in exceptions.Split(',', StringSplitOptions.RemoveEmptyEntries))
        {
            if (DateTime.TryParse(part.Trim(), out var date))
                dates.Add(date.Date);
        }

        return dates;
    }
}
