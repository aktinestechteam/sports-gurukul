using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class BookingApproval : BaseEntity
{
    public Guid BookingId { get; set; }
    public BookingApprovalStatus ApprovalStatus { get; set; } = BookingApprovalStatus.Pending;
    public Guid? ApproverUserId { get; set; }
    public DateTime? ReviewedOn { get; set; }
    public string? Comments { get; set; }
    public int EscalationLevel { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
