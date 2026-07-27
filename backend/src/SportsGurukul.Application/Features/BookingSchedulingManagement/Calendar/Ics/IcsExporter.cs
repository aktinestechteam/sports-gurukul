using System.Globalization;
using System.Text;
using SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;

namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Ics;

public class IcsExporter : ICalendarExporter
{
    public string Format => "ICS";

    public Task<byte[]> ExportAsync(
        IReadOnlyList<CalendarEvent> events,
        CalendarExportOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new CalendarExportOptions();
        var sb = new StringBuilder();

        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:" + (options.ProductIdentifier ?? "-//SportsGurukul//BookingCalendar//EN"));
        sb.AppendLine("CALSCALE:GREGORIAN");
        sb.AppendLine("METHOD:PUBLISH");
        sb.AppendLine("X-WR-CALNAME:SportsGurukul Bookings");

        if (!string.IsNullOrEmpty(options.TimeZone))
        {
            sb.AppendLine("X-WR-TIMEZONE:" + options.TimeZone);
        }

        foreach (var evt in events)
        {
            sb.AppendLine(BuildVevent(evt, options));
        }

        sb.AppendLine("END:VCALENDAR");

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return Task.FromResult(bytes);
    }

    public Task<byte[]> ExportSingleAsync(
        CalendarEvent calendarEvent,
        CalendarExportOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return ExportAsync(new[] { calendarEvent }, options, cancellationToken);
    }

    private static string BuildVevent(CalendarEvent evt, CalendarExportOptions options)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VEVENT");
        sb.AppendLine("UID:" + (string.IsNullOrEmpty(evt.Uid) ? Guid.NewGuid().ToString() : evt.Uid));
        sb.AppendLine("DTSTAMP:" + DateTime.UtcNow.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));

        if (evt.IsAllDay)
        {
            sb.AppendLine("DTSTART;VALUE=DATE:" + evt.StartDateTime.ToString("yyyyMMdd", CultureInfo.InvariantCulture));
            sb.AppendLine("DTEND;VALUE=DATE:" + evt.EndDateTime.AddDays(1).ToString("yyyyMMdd", CultureInfo.InvariantCulture));
        }
        else
        {
            sb.AppendLine("DTSTART:" + evt.StartDateTime.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
            sb.AppendLine("DTEND:" + evt.EndDateTime.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture));
        }

        sb.AppendLine("SUMMARY:" + EscapeIcsText(evt.Summary));

        if (options.IncludeDescription && !string.IsNullOrEmpty(evt.Description))
            sb.AppendLine("DESCRIPTION:" + EscapeIcsText(evt.Description));

        if (options.IncludeLocation && !string.IsNullOrEmpty(evt.Location))
            sb.AppendLine("LOCATION:" + EscapeIcsText(evt.Location));

        if (!string.IsNullOrEmpty(evt.Status))
            sb.AppendLine("STATUS:" + evt.Status.ToUpperInvariant());

        if (!string.IsNullOrEmpty(evt.Color))
            sb.AppendLine("COLOR:" + evt.Color);

        if (!string.IsNullOrEmpty(evt.Organizer))
            sb.AppendLine("ORGANIZER;CN=" + EscapeIcsText(evt.Organizer) + ":mailto:" + evt.Organizer);

        if (options.IncludeAttendees)
        {
            foreach (var attendee in evt.Attendees)
            {
                sb.AppendLine("ATTENDEE;ROLE=REQ-PARTICIPANT:mailto:" + attendee);
            }
        }

        if (!string.IsNullOrEmpty(evt.RecurrenceRule))
            sb.AppendLine("RRULE:" + evt.RecurrenceRule);

        if (options.ReminderMinutesBefore.HasValue)
        {
            sb.AppendLine("BEGIN:VALARM");
            sb.AppendLine("TRIGGER:-PT" + options.ReminderMinutesBefore.Value + "M");
            sb.AppendLine("ACTION:DISPLAY");
            sb.AppendLine("DESCRIPTION:Reminder");
            sb.AppendLine("END:VALARM");
        }

        sb.AppendLine("END:VEVENT");
        return sb.ToString();
    }

    private static string EscapeIcsText(string text)
    {
        if (string.IsNullOrEmpty(text)) return string.Empty;
        return text
            .Replace("\\", "\\\\")
            .Replace(";", "\\;")
            .Replace(",", "\\,")
            .Replace("\n", "\\n")
            .Replace("\r", "");
    }
}
