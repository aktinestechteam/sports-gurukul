namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

public class SavedBookingSearchDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public BookingSearchFilterDto Filters { get; set; } = new();
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
