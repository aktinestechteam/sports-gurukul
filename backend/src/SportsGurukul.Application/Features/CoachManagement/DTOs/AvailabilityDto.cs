namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class AvailabilityDto
{
    public Guid Id { get; set; }
    public string WeeklySchedule { get; set; } = string.Empty;
    public string TimeSlots { get; set; } = string.Empty;
    public bool OnlineAvailable { get; set; }
    public bool OfflineAvailable { get; set; }
    public int? TravelDistance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
