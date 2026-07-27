using System.Text;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.SharedScheduling.Engine;
using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public class CalendarEngine : ICalendarEngine
{
    private readonly ILogger<CalendarEngine> _logger;

    public CalendarEngine(ILogger<CalendarEngine> logger) => _logger = logger;

    public string GenerateIcsContent(
        string title, string description, TimeSlot slot,
        IReadOnlyList<string> attendeeEmails, string? location = null)
    {
        var uid = Guid.NewGuid().ToString("N") + "@sportsgurukul.com";
        var dtStart = slot.Date.Date + slot.StartTime;
        var dtEnd = slot.Date.Date + slot.EndTime;

        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//SportsGurukul//SchedulingEngine//EN");
        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{uid}");
        sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
        sb.AppendLine($"DTSTART:{dtStart:yyyyMMddTHHmmss}");
        sb.AppendLine($"DTEND:{dtEnd:yyyyMMddTHHmmss}");
        sb.AppendLine($"SUMMARY:{title}");

        if (!string.IsNullOrWhiteSpace(description))
            sb.AppendLine($"DESCRIPTION:{EscapeIcsText(description)}");

        if (!string.IsNullOrWhiteSpace(location))
            sb.AppendLine($"LOCATION:{EscapeIcsText(location)}");

        foreach (var email in attendeeEmails)
            sb.AppendLine($"ATTENDEE;ROLE=REQ-PARTICIPANT:mailto:{email}");

        sb.AppendLine("END:VEVENT");
        sb.AppendLine("END:VCALENDAR");

        var ics = sb.ToString();
        _logger.LogDebug("Generated ICS content for '{Title}' on {Date}", title, slot.Date.Date);
        return ics;
    }

    public string GenerateRecurringIcsContent(
        string title, string description, TimeSlot baseSlot,
        RecurrencePattern pattern, IReadOnlyList<string> attendeeEmails,
        string? location = null)
    {
        var uid = Guid.NewGuid().ToString("N") + "@sportsgurukul.com";
        var dtStart = baseSlot.Date.Date + baseSlot.StartTime;
        var dtEnd = baseSlot.Date.Date + baseSlot.EndTime;

        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//SportsGurukul//SchedulingEngine//EN");
        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine($"UID:{uid}");
        sb.AppendLine($"DTSTAMP:{DateTime.UtcNow:yyyyMMddTHHmmssZ}");
        sb.AppendLine($"DTSTART:{dtStart:yyyyMMddTHHmmss}");
        sb.AppendLine($"DTEND:{dtEnd:yyyyMMddTHHmmss}");
        sb.AppendLine($"SUMMARY:{title}");

        if (!string.IsNullOrWhiteSpace(description))
            sb.AppendLine($"DESCRIPTION:{EscapeIcsText(description)}");

        if (!string.IsNullOrWhiteSpace(location))
            sb.AppendLine($"LOCATION:{EscapeIcsText(location)}");

        var rrule = $"FREQ={pattern.Frequency.ToString().ToUpperInvariant()}";
        if (pattern.Interval > 1) rrule += $";INTERVAL={pattern.Interval}";
        if (pattern.MaxOccurrences.HasValue) rrule += $";COUNT={pattern.MaxOccurrences.Value}";
        if (pattern.EndDate.HasValue) rrule += $";UNTIL={pattern.EndDate.Value:yyyyMMdd}T235959Z";

        sb.AppendLine($"RRULE:{rrule}");

        foreach (var email in attendeeEmails)
            sb.AppendLine($"ATTENDEE;ROLE=REQ-PARTICIPANT:mailto:{email}");

        sb.AppendLine("END:VEVENT");
        sb.AppendLine("END:VCALENDAR");

        var ics = sb.ToString();
        _logger.LogDebug("Generated recurring ICS content for '{Title}' with frequency {Freq}", title, pattern.Frequency);
        return ics;
    }

    public CalendarEvent? ParseIcsEvent(string icsContent)
    {
        if (string.IsNullOrWhiteSpace(icsContent)) return null;

        var lines = icsContent.Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries);
        var inEvent = false;
        var uid = "";
        var summary = "";
        var description = "";
        var location = (string?)null;
        var dtStart = (string?)null;
        var dtEnd = (string?)null;
        var attendees = new List<string>();

        foreach (var line in lines)
        {
            if (line == "BEGIN:VEVENT") { inEvent = true; continue; }
            if (line == "END:VEVENT") { inEvent = false; continue; }
            if (!inEvent) continue;

            if (line.StartsWith("UID:")) uid = line[4..];
            else if (line.StartsWith("SUMMARY:")) summary = line[8..];
            else if (line.StartsWith("DESCRIPTION:")) description = UnescapeIcsText(line[12..]);
            else if (line.StartsWith("LOCATION:")) location = UnescapeIcsText(line[9..]);
            else if (line.StartsWith("DTSTART:")) dtStart = line[8..];
            else if (line.StartsWith("DTEND:")) dtEnd = line[6..];
            else if (line.Contains("ATTENDEE") && line.Contains("mailto:"))
            {
                var idx = line.IndexOf("mailto:", StringComparison.Ordinal);
                if (idx >= 0) attendees.Add(line[(idx + 7)..]);
            }
        }

        if (dtStart is null || dtEnd is null) return null;

        if (!DateTime.TryParseExact(dtStart, "yyyyMMddTHHmmss",
            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var startParsed))
            return null;

        if (!DateTime.TryParseExact(dtEnd, "yyyyMMddTHHmmss",
            System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out var endParsed))
            return null;

        var timeSlot = new TimeSlot
        {
            Date = startParsed.Date,
            StartTime = startParsed.TimeOfDay,
            EndTime = endParsed.TimeOfDay
        };

        _logger.LogDebug("Parsed ICS event: '{Summary}' on {Date}", summary, timeSlot.Date);

        return new CalendarEvent
        {
            Uid = uid,
            Summary = summary,
            Description = description,
            TimeSlot = timeSlot,
            Location = location,
            Attendees = attendees
        };
    }

    private static string EscapeIcsText(string text) =>
        text.Replace("\\", "\\\\").Replace(",", "\\,").Replace(";", "\\;").Replace("\n", "\\n");

    private static string UnescapeIcsText(string text) =>
        text.Replace("\\n", "\n").Replace("\\;", ";").Replace("\\,", ",").Replace("\\\\", "\\");
}
