using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Api.Common.Models;

public class CreateBookingApiRequest
{
    /// <summary>Type of booking.</summary>
    /// <example>TrainingSession</example>
    public BookingType BookingType { get; set; }

    /// <summary>Title of the booking.</summary>
    /// <example>Morning Badminton Session</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional description of the booking.</summary>
    /// <example>Weekly training session for U-14 group.</example>
    public string? Description { get; set; }

    /// <summary>Unique identifier of the academy this booking belongs to.</summary>
    /// <example>f47ac10b-58cc-4372-a567-0e02b2c3d479</example>
    public Guid AcademyId { get; set; }

    /// <summary>Optional branch identifier within the academy.</summary>
    /// <example>a1b2c3d4-e5f6-7890-abcd-ef1234567890</example>
    public Guid? BranchId { get; set; }

    /// <summary>Optional facility identifier for the booking.</summary>
    /// <example>b2c3d4e5-f6a7-8901-bcde-f12345678901</example>
    public Guid? FacilityId { get; set; }

    /// <summary>Optional coach identifier assigned to the booking.</summary>
    /// <example>c3d4e5f6-a7b8-9012-cdef-123456789012</example>
    public Guid? CoachId { get; set; }

    /// <summary>Optional athlete identifier for the booking.</summary>
    /// <example>d4e5f6a7-b8c9-0123-defa-234567890123</example>
    public Guid? AthleteId { get; set; }

    /// <summary>Optional training session identifier linked to the booking.</summary>
    /// <example>e5f6a7b8-c9d0-1234-efab-345678901234</example>
    public Guid? TrainingSessionId { get; set; }

    /// <summary>Date of the booking.</summary>
    /// <example>2026-08-15</example>
    public DateTime BookingDate { get; set; }

    /// <summary>Start time of the booking.</summary>
    /// <example>09:00:00</example>
    public TimeSpan StartTime { get; set; }

    /// <summary>End time of the booking.</summary>
    /// <example>10:30:00</example>
    public TimeSpan EndTime { get; set; }
}

public class UpdateBookingApiRequest
{
    /// <summary>Updated title of the booking.</summary>
    /// <example>Evening Badminton Session</example>
    public string? Title { get; set; }

    /// <summary>Updated description.</summary>
    /// <example>Rescheduled to evening slot.</example>
    public string? Description { get; set; }

    /// <summary>Updated booking date.</summary>
    /// <example>2026-08-16</example>
    public DateTime? BookingDate { get; set; }

    /// <summary>Updated start time.</summary>
    /// <example>17:00:00</example>
    public TimeSpan? StartTime { get; set; }

    /// <summary>Updated end time.</summary>
    /// <example>18:30:00</example>
    public TimeSpan? EndTime { get; set; }

    /// <summary>Updated facility identifier.</summary>
    public Guid? FacilityId { get; set; }

    /// <summary>Updated coach identifier.</summary>
    public Guid? CoachId { get; set; }

    /// <summary>Updated athlete identifier.</summary>
    public Guid? AthleteId { get; set; }
}

public class CancelBookingApiRequest
{
    /// <summary>Reason for cancellation (required).</summary>
    /// <example>Coach unavailable due to illness</example>
    public string Reason { get; set; } = string.Empty;

    /// <summary>Optional additional notes.</summary>
    /// <example>Will be rescheduled next week.</example>
    public string? Notes { get; set; }
}

public class RescheduleBookingApiRequest
{
    /// <summary>New date for the booking.</summary>
    /// <example>2026-08-20</example>
    public DateTime NewDate { get; set; }

    /// <summary>New start time.</summary>
    /// <example>10:00:00</example>
    public TimeSpan NewStartTime { get; set; }

    /// <summary>New end time.</summary>
    /// <example>11:30:00</example>
    public TimeSpan NewEndTime { get; set; }

    /// <summary>Optional reason for rescheduling.</summary>
    /// <example>Facility maintenance on original date</example>
    public string? Reason { get; set; }

    /// <summary>Optional notes for the reschedule.</summary>
    public string? Notes { get; set; }
}

public class CreateRecurringBookingApiRequest
{
    /// <summary>Type of booking.</summary>
    /// <example>TrainingSession</example>
    public BookingType BookingType { get; set; }

    /// <summary>Title of the recurring booking series.</summary>
    /// <example>Weekly Group Coaching</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>Optional description.</summary>
    public string? Description { get; set; }

    /// <summary>Academy identifier.</summary>
    public Guid AcademyId { get; set; }

    /// <summary>Optional branch identifier.</summary>
    public Guid? BranchId { get; set; }

    /// <summary>Optional facility identifier.</summary>
    public Guid? FacilityId { get; set; }

    /// <summary>Optional coach identifier.</summary>
    public Guid? CoachId { get; set; }

    /// <summary>Optional athlete identifier.</summary>
    public Guid? AthleteId { get; set; }

    /// <summary>Optional training session identifier.</summary>
    public Guid? TrainingSessionId { get; set; }

    /// <summary>Start date of the recurrence series.</summary>
    /// <example>2026-08-01</example>
    public DateTime StartDate { get; set; }

    /// <summary>Start time for each occurrence.</summary>
    /// <example>09:00:00</example>
    public TimeSpan StartTime { get; set; }

    /// <summary>End time for each occurrence.</summary>
    /// <example>10:30:00</example>
    public TimeSpan EndTime { get; set; }

    /// <summary>Recurrence frequency.</summary>
    /// <example>Weekly</example>
    public RecurrenceType RecurrenceType { get; set; }

    /// <summary>Optional number of occurrences.</summary>
    /// <example>12</example>
    public int? OccurrenceCount { get; set; }

    /// <summary>Optional end date for the recurrence.</summary>
    /// <example>2026-12-31</example>
    public DateTime? EndDate { get; set; }

    /// <summary>Optional iCalendar RRULE string for custom recurrence.</summary>
    public string? RRule { get; set; }

    /// <summary>Optional comma-separated dates to exclude from the series.</summary>
    public string? Exceptions { get; set; }
}

public class JoinWaitlistApiRequest
{
    /// <summary>User identifier to add to the waitlist.</summary>
    /// <example>d4e5f6a7-b8c9-0123-defa-234567890123</example>
    public Guid WaitlistUserId { get; set; }

    /// <summary>Optional notes for the waitlist entry.</summary>
    /// <example>Preferred morning slots.</example>
    public string? Notes { get; set; }
}

public class ResolveBookingConflictApiRequest
{
    /// <summary>Notes describing how the conflict was resolved.</summary>
    /// <example>Moved conflicting booking to alternate court.</example>
    public string ResolutionNotes { get; set; } = string.Empty;
}

public class ScheduleReminderApiRequest
{
    /// <summary>Minutes before the booking to send the reminder.</summary>
    /// <example>60</example>
    public int ReminderMinutesBefore { get; set; }

    /// <summary>Optional notification channel (e.g., Email, SMS, Push).</summary>
    /// <example>Email</example>
    public string? Channel { get; set; }

    /// <summary>Optional notes for the reminder.</summary>
    public string? Notes { get; set; }
}

public class SendReminderApiRequest
{
    /// <summary>Optional override channel to send the reminder through.</summary>
    /// <example>SMS</example>
    public string? OverrideChannel { get; set; }
}

public class ApprovalActionRequest
{
    /// <summary>Optional comments from the approver.</summary>
    /// <example>Approved for the upcoming tournament.</example>
    public string? Comments { get; set; }
}

public class RejectBookingApiRequest
{
    /// <summary>Optional reason for rejection.</summary>
    /// <example>Facility already reserved for maintenance.</example>
    public string? Reason { get; set; }
}
