using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EventParticipant : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? RegistrationId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public EventAttendanceStatus AttendanceStatus { get; set; } = EventAttendanceStatus.Registered;
    public string? Role { get; set; }
    public string? Organization { get; set; }
    public string? DietaryRequirements { get; set; }
    public string? SpecialNeeds { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public Athlete? Athlete { get; set; }
    public User? User { get; set; }
    public EventRegistration? Registration { get; set; }
}
