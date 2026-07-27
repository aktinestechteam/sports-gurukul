namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;

public interface ICalendarImporter
{
    string Format { get; }
    Task<IReadOnlyList<CalendarEvent>> ImportAsync(byte[] calendarData, CalendarImportOptions? options = null, CancellationToken cancellationToken = default);
}

public class CalendarImportOptions
{
    public string? TimeZone { get; set; }
    public DateTime? FilterAfter { get; set; }
    public DateTime? FilterBefore { get; set; }
    public bool IncludeAllDayEvents { get; set; } = true;
    public int MaxEvents { get; set; } = 1000;
}
