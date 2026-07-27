namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

public class BookingCalendarEventDto
{
    public Guid BookingId { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string BookingType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? FacilityName { get; set; }
    public string? CoachName { get; set; }
    public string? AthleteName { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string? TimeZone { get; set; }
    public string? Color { get; set; }
    public string? ResourceName { get; set; }
    public string? ResourceTypeName { get; set; }
}

public enum CalendarViewType
{
    Daily = 0,
    Weekly = 1,
    Monthly = 2,
    Agenda = 3
}

public class CalendarViewResultDto
{
    public CalendarViewType ViewType { get; set; }
    public DateTime ViewStartDate { get; set; }
    public DateTime ViewEndDate { get; set; }
    public IReadOnlyList<BookingCalendarEventDto> Events { get; set; } = [];
    public IReadOnlyList<CalendarDaySummaryDto> DaySummaries { get; set; } = [];
    public int TotalEvents { get; set; }
}

public class CalendarDaySummaryDto
{
    public DateTime Date { get; set; }
    public int EventCount { get; set; }
    public int TotalMinutesBooked { get; set; }
    public double UtilizationPercent { get; set; }
    public IReadOnlyList<string> FacilityNames { get; set; } = [];
}
