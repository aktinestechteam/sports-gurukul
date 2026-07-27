using SportsGurukul.Application.Features.SharedScheduling.Models;

namespace SportsGurukul.Application.Features.SharedScheduling.Engine;

public interface ICalendarEngine
{
    string GenerateIcsContent(string title, string description, TimeSlot slot, IReadOnlyList<string> attendeeEmails, string? location = null);
    string GenerateRecurringIcsContent(string title, string description, TimeSlot baseSlot, RecurrencePattern pattern, IReadOnlyList<string> attendeeEmails, string? location = null);
    CalendarEvent? ParseIcsEvent(string icsContent);
}

public sealed record CalendarEvent
{
    public string Uid { get; init; } = string.Empty;
    public string Summary { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public TimeSlot TimeSlot { get; init; } = null!;
    public string? Location { get; init; }
    public IReadOnlyList<string> Attendees { get; init; } = [];
    public RecurrencePattern? Recurrence { get; init; }
}
