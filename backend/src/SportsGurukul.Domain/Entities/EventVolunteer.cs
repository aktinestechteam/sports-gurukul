using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EventVolunteer : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? UserId { get; set; }
    public string VolunteerName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public EventVolunteerRole Role { get; set; } = EventVolunteerRole.General;
    public string? Assignment { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public User? User { get; set; }
}
