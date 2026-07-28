using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.EventManagement.Fixtures;

public static class EventDataFixture
{
    public static Event CreateDraftEvent(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventCode = "EVT-20260728-0001",
        EventName = "Summer Cricket Camp",
        Description = "A fun cricket camp for kids",
        ShortDescription = "Cricket camp",
        AcademyId = Guid.NewGuid(),
        SportId = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        Status = EventStatus.Draft,
        RegistrationType = EventRegistrationType.Free,
        StartDate = DateTime.UtcNow.AddDays(30),
        EndDate = DateTime.UtcNow.AddDays(37),
        RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
        RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
        MaxParticipants = 50,
        MinParticipants = 10,
        RegistrationFee = 0,
        IsFeatured = false,
        IsPublic = true,
        CreatedAt = DateTime.UtcNow
    };

    public static Event CreatePublishedEvent(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventCode = "EVT-20260728-0002",
        EventName = "Published Event",
        Description = "A published event",
        AcademyId = Guid.NewGuid(),
        SportId = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        Status = EventStatus.Published,
        RegistrationType = EventRegistrationType.Free,
        StartDate = DateTime.UtcNow.AddDays(30),
        EndDate = DateTime.UtcNow.AddDays(37),
        RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
        RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
        MaxParticipants = 50,
        IsPublic = true,
        CreatedAt = DateTime.UtcNow
    };

    public static Event CreateRegistrationOpenEvent(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventCode = "EVT-20260728-0003",
        EventName = "Registration Open Event",
        AcademyId = Guid.NewGuid(),
        SportId = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        Status = EventStatus.RegistrationOpen,
        RegistrationType = EventRegistrationType.Free,
        StartDate = DateTime.UtcNow.AddDays(30),
        EndDate = DateTime.UtcNow.AddDays(37),
        RegistrationOpenDate = DateTime.UtcNow.AddDays(-5),
        RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
        MaxParticipants = 50,
        IsPublic = true,
        Participants = new List<EventParticipant>(),
        Registrations = new List<EventRegistration>(),
        Sessions = new List<EventSession>(),
        Certificates = new List<EventCertificate>(),
        Feedbacks = new List<EventFeedback>(),
        Announcements = new List<EventAnnouncement>(),
        CreatedAt = DateTime.UtcNow
    };

    public static Event CreateCompletedEvent(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventCode = "EVT-20260728-0004",
        EventName = "Completed Event",
        AcademyId = Guid.NewGuid(),
        SportId = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        Status = EventStatus.Completed,
        RegistrationType = EventRegistrationType.Free,
        StartDate = DateTime.UtcNow.AddDays(-10),
        EndDate = DateTime.UtcNow.AddDays(-3),
        RegistrationOpenDate = DateTime.UtcNow.AddDays(-30),
        RegistrationCloseDate = DateTime.UtcNow.AddDays(-11),
        MaxParticipants = 50,
        IsPublic = true,
        Participants = new List<EventParticipant>(),
        Registrations = new List<EventRegistration>(),
        Sessions = new List<EventSession>(),
        Certificates = new List<EventCertificate>(),
        Feedbacks = new List<EventFeedback>(),
        Announcements = new List<EventAnnouncement>(),
        CreatedAt = DateTime.UtcNow.AddDays(-30)
    };

    public static Event CreateInProgressEvent(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventCode = "EVT-20260728-0005",
        EventName = "In Progress Event",
        AcademyId = Guid.NewGuid(),
        SportId = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        Status = EventStatus.InProgress,
        RegistrationType = EventRegistrationType.Free,
        StartDate = DateTime.UtcNow.AddDays(-5),
        EndDate = DateTime.UtcNow.AddDays(5),
        RegistrationOpenDate = DateTime.UtcNow.AddDays(-30),
        RegistrationCloseDate = DateTime.UtcNow.AddDays(-6),
        MaxParticipants = 50,
        IsPublic = true,
        Participants = new List<EventParticipant>(),
        Registrations = new List<EventRegistration>(),
        Sessions = new List<EventSession>(),
        Certificates = new List<EventCertificate>(),
        Feedbacks = new List<EventFeedback>(),
        Announcements = new List<EventAnnouncement>(),
        CreatedAt = DateTime.UtcNow.AddDays(-30)
    };

    public static Event CreateCancelledEvent(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventCode = "EVT-20260728-0006",
        EventName = "Cancelled Event",
        AcademyId = Guid.NewGuid(),
        SportId = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        Status = EventStatus.Cancelled,
        RegistrationType = EventRegistrationType.Free,
        StartDate = DateTime.UtcNow.AddDays(30),
        EndDate = DateTime.UtcNow.AddDays(37),
        RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
        RegistrationCloseDate = DateTime.UtcNow.AddDays(25),
        MaxParticipants = 50,
        IsPublic = true,
        CreatedAt = DateTime.UtcNow
    };

    public static Event CreateArchivedEvent(Guid? id = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventCode = "EVT-20260728-0007",
        EventName = "Archived Event",
        AcademyId = Guid.NewGuid(),
        SportId = Guid.NewGuid(),
        EventTypeId = Guid.NewGuid(),
        Status = EventStatus.Archived,
        RegistrationType = EventRegistrationType.Free,
        StartDate = DateTime.UtcNow.AddDays(-30),
        EndDate = DateTime.UtcNow.AddDays(-23),
        RegistrationOpenDate = DateTime.UtcNow.AddDays(-60),
        RegistrationCloseDate = DateTime.UtcNow.AddDays(-31),
        MaxParticipants = 50,
        IsPublic = true,
        CreatedAt = DateTime.UtcNow.AddDays(-60)
    };

    public static EventRegistration CreatePendingRegistration(Guid? id = null, Guid? eventId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        AthleteId = Guid.NewGuid(),
        RegistrationNumber = "REG-20260728-0001",
        Status = EventRegistrationStatus.Pending,
        RegistrationDate = DateTime.UtcNow,
        AmountPaid = 0,
        Event = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventRegistration CreateApprovedRegistration(Guid? id = null, Guid? eventId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        AthleteId = Guid.NewGuid(),
        RegistrationNumber = "REG-20260728-0002",
        Status = EventRegistrationStatus.Approved,
        ApprovalDate = DateTime.UtcNow,
        RegistrationDate = DateTime.UtcNow,
        AmountPaid = 0,
        Event = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventRegistration CreateWaitlistedRegistration(Guid? id = null, Guid? eventId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        AthleteId = Guid.NewGuid(),
        RegistrationNumber = "REG-20260728-0003",
        Status = EventRegistrationStatus.Waitlisted,
        WaitlistPosition = 1,
        RegistrationDate = DateTime.UtcNow,
        AmountPaid = 0,
        Event = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventRegistration CreateCancelledRegistration(Guid? id = null, Guid? eventId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        AthleteId = Guid.NewGuid(),
        RegistrationNumber = "REG-20260728-0004",
        Status = EventRegistrationStatus.Cancelled,
        RegistrationDate = DateTime.UtcNow,
        AmountPaid = 0,
        Event = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventParticipant CreateParticipant(Guid? id = null, Guid? eventId = null, EventAttendanceStatus status = EventAttendanceStatus.Registered) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        AthleteId = Guid.NewGuid(),
        ParticipantName = "John Doe",
        Email = "john@example.com",
        AttendanceStatus = status,
        Event = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventAttendance CreateAttendance(Guid? id = null, Guid? eventId = null, Guid? participantId = null, EventAttendanceStatus status = EventAttendanceStatus.CheckedIn) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        ParticipantId = participantId ?? Guid.NewGuid(),
        Status = status,
        CheckInTime = DateTime.UtcNow,
        MarkedBy = "System",
        Event = null!,
        Participant = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventCertificate CreateCertificate(Guid? id = null, Guid? eventId = null, Guid? participantId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        ParticipantId = participantId ?? Guid.NewGuid(),
        CertificateNumber = $"CERT-20260728-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
        CertificateType = "Participation",
        IssuedDate = DateTime.UtcNow,
        Event = null!,
        Participant = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventFeedback CreateFeedback(Guid? id = null, Guid? eventId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        OverallRating = EventFeedbackRating.Good,
        ContentRating = EventFeedbackRating.Good,
        Comments = "Great event!",
        WouldRecommend = true,
        Event = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventSession CreateSession(Guid? id = null, Guid? eventId = null) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        SessionCode = "SES-001",
        Title = "Opening Ceremony",
        SessionDate = DateTime.UtcNow.AddDays(1),
        StartTime = new TimeSpan(9, 0, 0),
        EndTime = new TimeSpan(10, 0, 0),
        Status = EventSessionStatus.Scheduled,
        Capacity = 50,
        CurrentAttendeeCount = 0,
        Event = null!,
        CreatedAt = DateTime.UtcNow
    };

    public static EventAnnouncement CreateAnnouncement(Guid? id = null, Guid? eventId = null, bool isPublished = true) => new()
    {
        Id = id ?? Guid.NewGuid(),
        EventId = eventId ?? Guid.NewGuid(),
        Title = "Important Update",
        Message = "Event schedule updated",
        IsPublished = isPublished,
        PublishedAt = isPublished ? DateTime.UtcNow : null,
        SendNotification = true,
        Priority = "High",
        Event = null!,
        CreatedAt = DateTime.UtcNow
    };
}
