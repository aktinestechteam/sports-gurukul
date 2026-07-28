namespace SportsGurukul.Application.Features.EventSearchDiscovery.DTOs;

public class CalendarEventDto
{
    public Guid Id { get; set; }
    public string EventCode { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? BannerUrl { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAllDay { get; set; }
    public string? VenueName { get; set; }
    public string? City { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public string Color { get; set; } = "#3B82F6";
    public int RegistrationCount { get; set; }
    public int? MaxParticipants { get; set; }
    public bool IsRegistrationOpen { get; set; }
}

public enum CalendarViewType
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Agenda = 3,
    Timeline = 4
}

public class CalendarDayDto
{
    public DateTime Date { get; set; }
    public int EventCount { get; set; }
    public IReadOnlyList<CalendarEventDto> Events { get; set; } = [];
    public bool HasEvents { get; set; }
}

public class CalendarMonthDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public IReadOnlyList<CalendarDayDto> Days { get; set; } = [];
    public int TotalEvents { get; set; }
}
