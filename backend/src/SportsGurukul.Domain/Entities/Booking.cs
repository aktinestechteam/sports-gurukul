using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Booking : BaseEntity
{
    public string BookingNumber { get; set; } = string.Empty;
    public BookingType BookingType { get; set; }
    public BookingStatus Status { get; set; } = BookingStatus.Draft;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? CoachId { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? TrainingSessionId { get; set; }
    public Guid? TournamentId { get; set; }
    public Guid? EventId { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Duration { get; set; }
    public BookingApprovalStatus ApprovalStatus { get; set; } = BookingApprovalStatus.Pending;
    public Guid? BookingCreatorId { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Academy Academy { get; set; } = null!;
    public AcademyBranch? Branch { get; set; }
    public Facility? Facility { get; set; }
    public Coach? Coach { get; set; }
    public Athlete? Athlete { get; set; }
    public TrainingSession? TrainingSession { get; set; }
    public ICollection<BookingItem> Items { get; set; } = new List<BookingItem>();
    public ICollection<BookingParticipant> Participants { get; set; } = new List<BookingParticipant>();
    public ICollection<BookingSchedule> Schedules { get; set; } = new List<BookingSchedule>();
    public ICollection<BookingRecurrence> Recurrences { get; set; } = new List<BookingRecurrence>();
    public ICollection<BookingWaitlist> WaitlistEntries { get; set; } = new List<BookingWaitlist>();
    public ICollection<BookingCancellation> Cancellations { get; set; } = new List<BookingCancellation>();
    public ICollection<BookingReschedule> Reschedules { get; set; } = new List<BookingReschedule>();
    public ICollection<BookingReminder> Reminders { get; set; } = new List<BookingReminder>();
    public ICollection<BookingApproval> Approvals { get; set; } = new List<BookingApproval>();
    public ICollection<BookingConflict> Conflicts { get; set; } = new List<BookingConflict>();
    public ICollection<BookingHistory> History { get; set; } = new List<BookingHistory>();
    public ICollection<BookingAttachment> Attachments { get; set; } = new List<BookingAttachment>();
}
