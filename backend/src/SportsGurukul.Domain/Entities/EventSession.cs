using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EventSession : BaseEntity
{
    public Guid EventId { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime SessionDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Guid? VenueId { get; set; }
    public Guid? SpeakerId { get; set; }
    public Guid? CoachId { get; set; }
    public EventSessionStatus Status { get; set; } = EventSessionStatus.Scheduled;
    public int? Capacity { get; set; }
    public int CurrentAttendeeCount { get; set; }
    public bool IsBreak { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public EventVenue? Venue { get; set; }
    public EventSpeaker? Speaker { get; set; }
    public EventCoach? Coach { get; set; }
    public ICollection<EventAttendance> Attendances { get; set; } = new List<EventAttendance>();
}
