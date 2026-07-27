using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class TrainingSession : BaseEntity
{
    public Guid BatchId { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public string SessionTitle { get; set; } = string.Empty;
    public SessionType SessionType { get; set; } = SessionType.Practice;
    public DateTime SessionDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid CoachId { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Scheduled;
    public byte[] RowVersion { get; set; } = [];

    public TrainingBatch Batch { get; set; } = null!;
    public Facility? Facility { get; set; }
    public Coach Coach { get; set; } = null!;
    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    public ICollection<TrainingAssessment> Assessments { get; set; } = new List<TrainingAssessment>();
    public ICollection<SessionSchedule> Schedules { get; set; } = new List<SessionSchedule>();
    public ICollection<TrainingMaterial> Materials { get; set; } = new List<TrainingMaterial>();
}
