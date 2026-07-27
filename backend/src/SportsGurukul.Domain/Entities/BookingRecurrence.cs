using SportsGurukul.Domain.Common;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Domain.Entities;

public class BookingRecurrence : BaseEntity
{
    public Guid BookingId { get; set; }
    public RecurrenceType RecurrenceType { get; set; }
    public string? RRule { get; set; }
    public DateTime? EndDate { get; set; }
    public int? OccurrenceCount { get; set; }
    public string? Exceptions { get; set; }
    public bool IsActive { get; set; } = true;
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
