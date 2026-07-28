using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EventFeedback : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? ParticipantId { get; set; }
    public Guid? UserId { get; set; }
    public EventFeedbackRating OverallRating { get; set; }
    public EventFeedbackRating? ContentRating { get; set; }
    public EventFeedbackRating? SpeakerRating { get; set; }
    public EventFeedbackRating? VenueRating { get; set; }
    public EventFeedbackRating? OrganizationRating { get; set; }
    public string? Comments { get; set; }
    public string? Suggestions { get; set; }
    public bool WouldRecommend { get; set; }
    public bool IsAnonymous { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public EventParticipant? Participant { get; set; }
    public User? User { get; set; }
}
