using SportsGurukul.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace SportsGurukul.Api.Common.Models.SwaggerExamples;

#region Request Examples

public class CreateBookingApiRequestExample : IExamplesProvider<CreateBookingApiRequest>
{
    public CreateBookingApiRequest GetExamples() => new()
    {
        BookingType = BookingType.TrainingSession,
        Title = "Morning Badminton Session",
        Description = "Weekly group training for U-14 athletes.",
        AcademyId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        BranchId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890"),
        FacilityId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        CoachId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
        AthleteId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
        BookingDate = new DateTime(2026, 8, 15),
        StartTime = new TimeSpan(9, 0, 0),
        EndTime = new TimeSpan(10, 30, 0)
    };
}

public class UpdateBookingApiRequestExample : IExamplesProvider<UpdateBookingApiRequest>
{
    public UpdateBookingApiRequest GetExamples() => new()
    {
        Title = "Evening Badminton Session",
        Description = "Rescheduled to evening slot due to facility maintenance.",
        BookingDate = new DateTime(2026, 8, 16),
        StartTime = new TimeSpan(17, 0, 0),
        EndTime = new TimeSpan(18, 30, 0)
    };
}

public class CancelBookingApiRequestExample : IExamplesProvider<CancelBookingApiRequest>
{
    public CancelBookingApiRequest GetExamples() => new()
    {
        Reason = "Coach unavailable due to illness",
        Notes = "Will be rescheduled next week."
    };
}

public class RescheduleBookingApiRequestExample : IExamplesProvider<RescheduleBookingApiRequest>
{
    public RescheduleBookingApiRequest GetExamples() => new()
    {
        NewDate = new DateTime(2026, 8, 20),
        NewStartTime = new TimeSpan(10, 0, 0),
        NewEndTime = new TimeSpan(11, 30, 0),
        Reason = "Facility maintenance on original date",
        Notes = "Court 2 will be available on the new date."
    };
}

public class CreateRecurringBookingApiRequestExample : IExamplesProvider<CreateRecurringBookingApiRequest>
{
    public CreateRecurringBookingApiRequest GetExamples() => new()
    {
        BookingType = BookingType.GroupCoaching,
        Title = "Weekly Group Coaching",
        Description = "Recurring weekly group coaching session.",
        AcademyId = Guid.Parse("f47ac10b-58cc-4372-a567-0e02b2c3d479"),
        FacilityId = Guid.Parse("b2c3d4e5-f6a7-8901-bcde-f12345678901"),
        CoachId = Guid.Parse("c3d4e5f6-a7b8-9012-cdef-123456789012"),
        StartDate = new DateTime(2026, 8, 1),
        StartTime = new TimeSpan(9, 0, 0),
        EndTime = new TimeSpan(10, 30, 0),
        RecurrenceType = RecurrenceType.Weekly,
        OccurrenceCount = 12,
        EndDate = new DateTime(2026, 10, 31)
    };
}

public class JoinWaitlistApiRequestExample : IExamplesProvider<JoinWaitlistApiRequest>
{
    public JoinWaitlistApiRequest GetExamples() => new()
    {
        WaitlistUserId = Guid.Parse("d4e5f6a7-b8c9-0123-defa-234567890123"),
        Notes = "Preferred morning slots."
    };
}

public class ResolveBookingConflictApiRequestExample : IExamplesProvider<ResolveBookingConflictApiRequest>
{
    public ResolveBookingConflictApiRequest GetExamples() => new()
    {
        ResolutionNotes = "Moved conflicting booking to alternate court."
    };
}

public class ScheduleReminderApiRequestExample : IExamplesProvider<ScheduleReminderApiRequest>
{
    public ScheduleReminderApiRequest GetExamples() => new()
    {
        ReminderMinutesBefore = 60,
        Channel = "Email",
        Notes = "Reminder for morning training session."
    };
}

public class SendReminderApiRequestExample : IExamplesProvider<SendReminderApiRequest>
{
    public SendReminderApiRequest GetExamples() => new()
    {
        OverrideChannel = "SMS"
    };
}

public class ApprovalActionRequestExample : IExamplesProvider<ApprovalActionRequest>
{
    public ApprovalActionRequest GetExamples() => new()
    {
        Comments = "Approved for the upcoming tournament."
    };
}

public class RejectBookingApiRequestExample : IExamplesProvider<RejectBookingApiRequest>
{
    public RejectBookingApiRequest GetExamples() => new()
    {
        Reason = "Facility already reserved for maintenance."
    };
}

#endregion
