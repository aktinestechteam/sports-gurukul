using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class BookingConflict : BaseEntity
{
    public Guid BookingId { get; set; }
    public Guid ConflictingBookingId { get; set; }
    public BookingConflictType ConflictType { get; set; }
    public string? Description { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedOn { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
