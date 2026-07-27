namespace SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

public class BookingSummaryDto
{
    public Guid Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string BookingType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public Guid AcademyId { get; set; }
    public string? AcademyName { get; set; }
    public string? FacilityName { get; set; }
    public string? CoachName { get; set; }
    public string? AthleteName { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Duration { get; set; }
    public string ApprovalStatus { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
