namespace SportsGurukul.Application.Features.BookingSchedulingManagement.DTOs;

public class BookingDto
{
    public Guid Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string BookingType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AcademyId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? FacilityId { get; set; }
    public Guid? CoachId { get; set; }
    public Guid? AthleteId { get; set; }
    public Guid? TrainingSessionId { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Duration { get; set; }
    public string ApprovalStatus { get; set; } = string.Empty;
    public Guid? BookingCreatorId { get; set; }
    public string? AcademyName { get; set; }
    public string? FacilityName { get; set; }
    public string? CoachName { get; set; }
    public string? AthleteName { get; set; }
    public IReadOnlyList<BookingScheduleDto> Schedules { get; set; } = [];
    public IReadOnlyList<BookingParticipantDto> Participants { get; set; } = [];
    public IReadOnlyList<BookingItemDto> Items { get; set; } = [];
    public IReadOnlyList<BookingReminderDto> Reminders { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BookingParticipantDto
{
    public Guid Id { get; set; }
    public Guid ParticipantId { get; set; }
    public string? ParticipantName { get; set; }
    public string? Role { get; set; }
    public bool Confirmed { get; set; }
    public bool Attended { get; set; }
}

public class BookingItemDto
{
    public Guid Id { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? ItemDescription { get; set; }
    public int Quantity { get; set; }
    public string? Unit { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalPrice { get; set; }
}
