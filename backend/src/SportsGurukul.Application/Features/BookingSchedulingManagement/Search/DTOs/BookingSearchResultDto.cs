namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

public class BookingSearchResultDto
{
    public Guid Id { get; set; }
    public string BookingNumber { get; set; } = string.Empty;
    public string BookingType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid AcademyId { get; set; }
    public string? AcademyName { get; set; }
    public Guid? BranchId { get; set; }
    public string? BranchName { get; set; }
    public Guid? FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public Guid? CoachId { get; set; }
    public string? CoachName { get; set; }
    public Guid? AthleteId { get; set; }
    public string? AthleteName { get; set; }
    public DateTime BookingDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int Duration { get; set; }
    public string ApprovalStatus { get; set; } = string.Empty;
    public Guid? BookingCreatorId { get; set; }
    public int ParticipantCount { get; set; }
    public bool HasConflict { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class BookingSearchPageResultDto
{
    public IReadOnlyList<BookingSearchResultDto> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public double SearchTimeMs { get; set; }
}
