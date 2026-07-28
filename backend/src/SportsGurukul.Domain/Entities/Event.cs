using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class Event : BaseEntity
{
    public string EventCode { get; set; } = string.Empty;
    public string EventName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public Guid AcademyId { get; set; }
    public Guid SportId { get; set; }
    public Guid EventTypeId { get; set; }
    public Guid? EventCategoryId { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    public EventRegistrationType RegistrationType { get; set; } = EventRegistrationType.Free;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public DateTime RegistrationOpenDate { get; set; }
    public DateTime RegistrationCloseDate { get; set; }
    public int? MaxParticipants { get; set; }
    public int? MinParticipants { get; set; }
    public decimal? RegistrationFee { get; set; }
    public bool IsFeatured { get; set; }
    public bool IsPublic { get; set; } = true;
    public string? BannerUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? Website { get; set; }
    public string? Tags { get; set; }
    public string? Requirements { get; set; }
    public string? WhatToBring { get; set; }
    public string? CancellationPolicy { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Academy Academy { get; set; } = null!;
    public Sport Sport { get; set; } = null!;
    public EventTypeEntity EventType { get; set; } = null!;
    public EventCategory? EventCategory { get; set; }
    public ICollection<EventSchedule> Schedules { get; set; } = new List<EventSchedule>();
    public ICollection<EventVenue> Venues { get; set; } = new List<EventVenue>();
    public ICollection<EventRegistration> Registrations { get; set; } = new List<EventRegistration>();
    public ICollection<EventParticipant> Participants { get; set; } = new List<EventParticipant>();
    public ICollection<EventSpeaker> Speakers { get; set; } = new List<EventSpeaker>();
    public ICollection<EventCoach> Coaches { get; set; } = new List<EventCoach>();
    public ICollection<EventVolunteer> Volunteers { get; set; } = new List<EventVolunteer>();
    public ICollection<EventSponsor> Sponsors { get; set; } = new List<EventSponsor>();
    public ICollection<EventSession> Sessions { get; set; } = new List<EventSession>();
    public ICollection<EventAgenda> Agendas { get; set; } = new List<EventAgenda>();
    public ICollection<EventTicket> Tickets { get; set; } = new List<EventTicket>();
    public ICollection<EventAttendance> Attendances { get; set; } = new List<EventAttendance>();
    public ICollection<EventCertificate> Certificates { get; set; } = new List<EventCertificate>();
    public ICollection<EventFeedback> Feedbacks { get; set; } = new List<EventFeedback>();
    public ICollection<EventMedia> Media { get; set; } = new List<EventMedia>();
    public ICollection<EventDocument> Documents { get; set; } = new List<EventDocument>();
    public ICollection<EventAnnouncement> Announcements { get; set; } = new List<EventAnnouncement>();
}
