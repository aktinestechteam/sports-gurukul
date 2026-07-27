namespace SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

public class BookingHistoryDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? PreviousValue { get; set; }
    public string? NewValue { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime PerformedOn { get; set; }
    public string? Notes { get; set; }
}
