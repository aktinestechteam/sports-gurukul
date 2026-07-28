using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EventAttendance : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? SessionId { get; set; }
    public Guid ParticipantId { get; set; }
    public EventAttendanceStatus Status { get; set; } = EventAttendanceStatus.Registered;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Remarks { get; set; }
    public string? MarkedBy { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public EventSession? Session { get; set; }
    public EventParticipant Participant { get; set; } = null!;
}
