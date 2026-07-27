namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;

public interface ICalendarProvider
{
    string ProviderName { get; }
    bool IsAvailable { get; }
    Task<CalendarConnectionResult> TestConnectionAsync(CalendarConnectionSettings settings, CancellationToken cancellationToken = default);
}

public class CalendarConnectionSettings
{
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public string? CalendarId { get; set; }
    public string? ServerUrl { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? ApiKey { get; set; }
}

public class CalendarConnectionResult
{
    public bool IsConnected { get; set; }
    public string? Message { get; set; }
    public string? CalendarName { get; set; }
    public int? EventCount { get; set; }
}
