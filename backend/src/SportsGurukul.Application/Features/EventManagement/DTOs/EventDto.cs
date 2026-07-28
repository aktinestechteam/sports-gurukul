using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.EventManagement.DTOs;

public class EventDto
{
    public Guid Id { get; set; }
    public string EventCode { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public Guid AcademyId { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public Guid SportId { get; set; }
    public string SportName { get; set; } = string.Empty;
    public Guid EventTypeId { get; set; }
    public string EventTypeName { get; set; } = string.Empty;
    public Guid? EventCategoryId { get; set; }
    public string? EventCategoryName { get; set; }
    public string Status { get; set; } = string.Empty;
    public string RegistrationType { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationOpenDate { get; set; }
    public DateTime RegistrationCloseDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int? MinParticipants { get; set; }
    public decimal? RegistrationFee { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublic { get; set; }
    public string? BannerUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public string? Tags { get; set; }
    public string? Requirements { get; set; }
    public string? WhatToBring { get; set; }
    public string? CancellationPolicy { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<EventScheduleDto> Schedules { get; set; } = [];
    public List<EventVenueDto> Venues { get; set; } = [];
    public List<EventSessionDto> Sessions { get; set; } = [];
    public int RegistrationCount { get; set; }
    public int ParticipantCount { get; set; }
    public double AverageFeedbackScore { get; set; }

    public static EventDto MapToDto(
        Event @event,
        string? academyName = null,
        string? sportName = null,
        string? eventTypeName = null,
        string? eventCategoryName = null,
        List<EventScheduleDto>? schedules = null,
        List<EventVenueDto>? venues = null,
        List<EventSessionDto>? sessions = null,
        int registrationCount = 0,
        int participantCount = 0,
        double averageFeedbackScore = 0)
    {
        return new EventDto
        {
            Id = @event.Id,
            EventCode = @event.EventCode,
            EventName = @event.EventName,
            Description = @event.Description,
            ShortDescription = @event.ShortDescription,
            AcademyId = @event.AcademyId,
            AcademyName = academyName ?? string.Empty,
            SportId = @event.SportId,
            SportName = sportName ?? string.Empty,
            EventTypeId = @event.EventTypeId,
            EventTypeName = eventTypeName ?? string.Empty,
            EventCategoryId = @event.EventCategoryId,
            EventCategoryName = eventCategoryName,
            Status = @event.Status.ToString(),
            RegistrationType = @event.RegistrationType.ToString(),
            StartDate = @event.StartDate,
            EndDate = @event.EndDate,
            RegistrationOpenDate = @event.RegistrationOpenDate,
            RegistrationCloseDate = @event.RegistrationCloseDate,
            MaxParticipants = @event.MaxParticipants,
            MinParticipants = @event.MinParticipants,
            RegistrationFee = @event.RegistrationFee,
            IsFeatured = @event.IsFeatured,
            IsPublic = @event.IsPublic,
            BannerUrl = @event.BannerUrl,
            ContactEmail = @event.ContactEmail,
            ContactPhone = @event.ContactPhone,
            Website = @event.Website,
            Tags = @event.Tags,
            Requirements = @event.Requirements,
            WhatToBring = @event.WhatToBring,
            CancellationPolicy = @event.CancellationPolicy,
            RowVersion = @event.RowVersion,
            CreatedAt = @event.CreatedAt,
            UpdatedAt = @event.UpdatedAt,
            Schedules = schedules ?? [],
            Venues = venues ?? [],
            Sessions = sessions ?? [],
            RegistrationCount = registrationCount,
            ParticipantCount = participantCount,
            AverageFeedbackScore = averageFeedbackScore
        };
    }
}

public class EventSummaryDto
{
    public Guid Id { get; set; }
    public string EventCode { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public string AcademyName { get; set; } = string.Empty;
    public string SportName { get; set; } = string.Empty;
    public string EventTypeName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int RegistrationCount { get; set; }
    public decimal? RegistrationFee { get; set; }
    public bool IsFeatured { get; set; }
    public string? BannerUrl { get; set; }
}

public class RegistrationDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public Guid? AthleteId { get; set; }
    public Guid? UserId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal? AmountPaid { get; set; }
    public string? PaymentReference { get; set; }
    public string? Notes { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public int? WaitlistPosition { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public static class RegistrationDtoExtensions
{
    internal static RegistrationDto MapToDto(Domain.Entities.EventRegistration reg, string eventName = "")
    {
        return new RegistrationDto
        {
            Id = reg.Id,
            EventId = reg.EventId,
            EventName = eventName,
            AthleteId = reg.AthleteId,
            UserId = reg.UserId,
            RegistrationNumber = reg.RegistrationNumber,
            Status = reg.Status.ToString(),
            AmountPaid = reg.AmountPaid,
            PaymentReference = reg.PaymentReference,
            Notes = reg.Notes,
            RegistrationDate = reg.RegistrationDate,
            ApprovalDate = reg.ApprovalDate,
            RejectionReason = reg.RejectionReason,
            WaitlistPosition = reg.WaitlistPosition,
            RowVersion = reg.RowVersion,
            CreatedAt = reg.CreatedAt
        };
    }
}

public class AttendanceDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid? SessionId { get; set; }
    public string? SessionTitle { get; set; }
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; }
    public string? Remarks { get; set; }
    public string? MarkedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class EventSessionDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string SessionCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime SessionDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public Guid? VenueId { get; set; }
    public string? VenueName { get; set; }
    public Guid? SpeakerId { get; set; }
    public string? SpeakerName { get; set; }
    public Guid? CoachId { get; set; }
    public string? CoachName { get; set; }
    public string Status { get; set; } = string.Empty;
    public int? Capacity { get; set; }
    public int CurrentAttendeeCount { get; set; }
    public bool IsBreak { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class EventScheduleDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public bool IsAllDay { get; set; }
    public string? RecurrenceRule { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class EventVenueDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public Guid? FacilityId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int? Capacity { get; set; }
    public string? MapUrl { get; set; }
    public string? Instructions { get; set; }
    public bool IsPrimary { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CertificateDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public Guid ParticipantId { get; set; }
    public string ParticipantName { get; set; } = string.Empty;
    public string CertificateNumber { get; set; } = string.Empty;
    public string? CertificateType { get; set; }
    public DateTime IssuedDate { get; set; }
    public string? IssuedBy { get; set; }
    public string? DocumentUrl { get; set; }
    public bool IsPrinted { get; set; }
    public bool IsSent { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FeedbackDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public Guid? ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public Guid? UserId { get; set; }
    public string OverallRating { get; set; } = string.Empty;
    public string? ContentRating { get; set; }
    public string? SpeakerRating { get; set; }
    public string? VenueRating { get; set; }
    public string? OrganizationRating { get; set; }
    public string? Comments { get; set; }
    public string? Suggestions { get; set; }
    public bool WouldRecommend { get; set; }
    public bool IsAnonymous { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class AnnouncementDto
{
    public Guid Id { get; set; }
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsPublished { get; set; }
    public DateTime? PublishedAt { get; set; }
    public bool SendNotification { get; set; }
    public string? Priority { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class StatisticsDto
{
    public Guid EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int TotalRegistrations { get; set; }
    public int ApprovedRegistrations { get; set; }
    public int PendingRegistrations { get; set; }
    public int CancelledRegistrations { get; set; }
    public int WaitlistedRegistrations { get; set; }
    public int TotalParticipants { get; set; }
    public int CheckedInCount { get; set; }
    public int PresentCount { get; set; }
    public double AttendanceRate { get; set; }
    public double CompletionRate { get; set; }
    public int CertificatesIssued { get; set; }
    public double AverageFeedbackScore { get; set; }
    public int FeedbackCount { get; set; }
    public int TotalSessions { get; set; }
    public int CompletedSessions { get; set; }
    public decimal TotalRevenue { get; set; }
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
