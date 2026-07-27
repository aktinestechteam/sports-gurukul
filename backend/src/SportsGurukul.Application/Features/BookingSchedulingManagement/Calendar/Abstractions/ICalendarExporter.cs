namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;

public interface ICalendarExporter
{
    string Format { get; }
    Task<byte[]> ExportAsync(IReadOnlyList<CalendarEvent> events, CalendarExportOptions? options = null, CancellationToken cancellationToken = default);
    Task<byte[]> ExportSingleAsync(CalendarEvent calendarEvent, CalendarExportOptions? options = null, CancellationToken cancellationToken = default);
}

public class CalendarExportOptions
{
    public string? TimeZone { get; set; }
    public string? ProductIdentifier { get; set; }
    public bool IncludeDescription { get; set; } = true;
    public bool IncludeLocation { get; set; } = true;
    public bool IncludeAttendees { get; set; } = true;
    public bool IncludeAttachments { get; set; } = false;
    public int? ReminderMinutesBefore { get; set; }
    public string? DefaultStatus { get; set; }
}
