using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class EventRegistration : BaseEntity
{
    public Guid EventId { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? UserId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public EventRegistrationStatus Status { get; set; } = EventRegistrationStatus.Pending;
    public decimal? AmountPaid { get; set; }
    public string? PaymentReference { get; set; }
    public string? Notes { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string? RejectionReason { get; set; }
    public int? WaitlistPosition { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Event Event { get; set; } = null!;
    public Athlete? Athlete { get; set; }
    public User? User { get; set; }
}
