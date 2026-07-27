namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Calendar.Abstractions;

public class CalendarEvent
{
    public Guid Id { get; set; }
    public string Uid { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public bool IsAllDay { get; set; }
    public string? TimeZone { get; set; }
    public string? Organizer { get; set; }
    public IReadOnlyList<string> Attendees { get; set; } = [];
    public string? RecurrenceRule { get; set; }
    public string? Status { get; set; }
    public string? Color { get; set; }
    public IReadOnlyList<CalendarEventAttachment> Attachments { get; set; } = [];
    public IReadOnlyDictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();
}

public class CalendarEventAttachment
{
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? ContentType { get; set; }
    public long? SizeBytes { get; set; }
}
