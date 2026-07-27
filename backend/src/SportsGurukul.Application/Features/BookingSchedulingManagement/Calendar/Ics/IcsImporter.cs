using System.Globalization;
using System.Text;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Ics;

public class IcsImporter : ICalendarImporter
{
    public string Format => "ICS";

    public Task<IReadOnlyList<CalendarEvent>> ImportAsync(
        byte[] calendarData,
        CalendarImportOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new CalendarImportOptions();
        var content = Encoding.UTF8.GetString(calendarData);
        var events = ParseVcalendar(content, options);
        return Task.FromResult<IReadOnlyList<CalendarEvent>>(events);
    }

    private static List<CalendarEvent> ParseVcalendar(string content, CalendarImportOptions options)
    {
        var events = new List<CalendarEvent>();
        var lines = NormalizeLines(content.Split(["\r\n", "\r", "\n"], StringSplitOptions.None));
        var inEvent = false;
        CalendarEvent? current = null;

        foreach (var line in lines)
        {
            if (line == "BEGIN:VEVENT")
            {
                inEvent = true;
                current = new CalendarEvent { Uid = Guid.NewGuid().ToString() };
                continue;
            }

            if (line == "END:VEVENT")
            {
                if (current is not null)
                {
                    if (events.Count < options.MaxEvents &&
                        PassesFilter(current, options))
                    {
                        events.Add(current);
                    }
                }
                inEvent = false;
                current = null;
                continue;
            }

            if (!inEvent || current is null) continue;

            if (line.StartsWith("UID:", StringComparison.OrdinalIgnoreCase))
                current.Uid = UnescapeIcsText(line[4..]);
            else if (line.StartsWith("SUMMARY:", StringComparison.OrdinalIgnoreCase))
                current.Summary = UnescapeIcsText(line[8..]);
            else if (line.StartsWith("DESCRIPTION:", StringComparison.OrdinalIgnoreCase))
                current.Description = UnescapeIcsText(line[12..]);
            else if (line.StartsWith("LOCATION:", StringComparison.OrdinalIgnoreCase))
                current.Location = UnescapeIcsText(line[9..]);
            else if (line.StartsWith("STATUS:", StringComparison.OrdinalIgnoreCase))
                current.Status = line[7..];
            else if (line.StartsWith("COLOR:", StringComparison.OrdinalIgnoreCase))
                current.Color = line[6..];
            else if (line.StartsWith("ORGANIZER", StringComparison.OrdinalIgnoreCase))
            {
                var idx = line.IndexOf("mailto:", StringComparison.OrdinalIgnoreCase);
                if (idx >= 0) current.Organizer = line[(idx + 7)..];
            }
            else if (line.StartsWith("ATTENDEE", StringComparison.OrdinalIgnoreCase))
            {
                var idx = line.IndexOf("mailto:", StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                    current.Attendees = current.Attendees.Append(line[(idx + 7)..]).ToList();
            }
            else if (line.StartsWith("RRULE:", StringComparison.OrdinalIgnoreCase))
                current.RecurrenceRule = line[6..];
            else if (line.StartsWith("DTSTART", StringComparison.OrdinalIgnoreCase))
            {
                var dt = ParseIcsDateTime(line);
                if (dt.HasValue)
                {
                    current.StartDateTime = dt.Value;
                    current.IsAllDay = line.Contains("VALUE=DATE", StringComparison.OrdinalIgnoreCase) &&
                                       !line.Contains("VALUE=DATE-TIME", StringComparison.OrdinalIgnoreCase);
                }
            }
            else if (line.StartsWith("DTEND", StringComparison.OrdinalIgnoreCase))
            {
                var dt = ParseIcsDateTime(line);
                if (dt.HasValue) current.EndDateTime = dt.Value;
            }
        }

        return events;
    }

    private static DateTime? ParseIcsDateTime(string line)
    {
        var value = line.Contains(':') ? line[(line.IndexOf(':') + 1)..] : line;

        if (DateTime.TryParseExact(value.Trim(),
            ["yyyyMMddTHHmmssZ", "yyyyMMddTHHmmss", "yyyyMMdd"],
            CultureInfo.InvariantCulture,
            DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
            out var dt))
        {
            return dt;
        }

        return null;
    }

    private static bool PassesFilter(CalendarEvent evt, CalendarImportOptions options)
    {
        if (!options.IncludeAllDayEvents && evt.IsAllDay) return false;
        if (options.FilterAfter.HasValue && evt.EndDateTime < options.FilterAfter.Value) return false;
        if (options.FilterBefore.HasValue && evt.StartDateTime > options.FilterBefore.Value) return false;
        return true;
    }

    private static string[] NormalizeLines(string[] lines)
    {
        var result = new List<string>();
        foreach (var line in lines)
        {
            if (line.StartsWith(' ') || line.StartsWith('\t'))
            {
                if (result.Count > 0)
                    result[^1] += line[1..];
            }
            else
            {
                result.Add(line);
            }
        }
        return result.ToArray();
    }

    private static string UnescapeIcsText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text
            .Replace("\\n", "\n")
            .Replace("\\,", ",")
            .Replace("\\;", ";")
            .Replace("\\\\", "\\");
    }
}
