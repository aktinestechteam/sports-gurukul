using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class BookingParticipant : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public string? Role { get; set; }
    public bool Confirmed { get; set; }
    public bool Attended { get; set; }
    public string? Notes { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
