namespace SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

public class WaitlistDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public Guid WaitlistUserId { get; set; }
    public int Priority { get; set; }
    public DateTime RequestedOn { get; set; }
    public int PromotionOrder { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
}
