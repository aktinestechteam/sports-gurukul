namespace SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

public class BookingScheduleDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsCancelled { get; set; }
    public string? CancellationReason { get; set; }
    public string? Notes { get; set; }
}
