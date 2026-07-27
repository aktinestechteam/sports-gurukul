using SportsGurukul.Domain.Common;

namespace SportsGurukul.Domain.Entities;

public class BookingAttachment : BaseEntity
{
    public Guid BookingId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string? FileType { get; set; }
    public long FileSize { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string? Description { get; set; }
    public byte[] RowVersion { get; set; } = [];

    public Booking Booking { get; set; } = null!;
}
