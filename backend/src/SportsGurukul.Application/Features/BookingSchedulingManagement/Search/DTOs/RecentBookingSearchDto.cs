namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

public class RecentBookingSearchDto
{
    public Guid Id { get; set; }
    public string QueryText { get; set; } = string.Empty;
    public BookingSearchFilterDto? Filters { get; set; }
    public int ResultCount { get; set; }
    public DateTime SearchedAt { get; set; }
}
