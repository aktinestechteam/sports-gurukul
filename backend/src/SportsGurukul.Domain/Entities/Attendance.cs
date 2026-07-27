using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Attendance : BaseEntity
{
    public Guid SessionId { get; set; }
    public Guid AthleteId { get; set; }
    public AttendanceStatus AttendanceStatus { get; set; } = AttendanceStatus.Absent;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Remarks { get; set; }

    public TrainingSession Session { get; set; } = null!;
    public Athlete Athlete { get; set; } = null!;
}
