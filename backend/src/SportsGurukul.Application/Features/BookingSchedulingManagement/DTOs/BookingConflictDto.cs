namespace SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

public class BookingConflictDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid ConflictingBookingId { get; set; }
    public string ConflictType { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolutionNotes { get; set; }
    public DateTime? ResolvedOn { get; set; }
    public DateTime CreatedAt { get; set; }
}
