namespace SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

public class ReminderDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public int ReminderMinutesBefore { get; set; }
    public DateTime ScheduledAt { get; set; }
    public bool IsSent { get; set; }
    public DateTime? SentAt { get; set; }
    public string? Channel { get; set; }
    public string? Notes { get; set; }
}
