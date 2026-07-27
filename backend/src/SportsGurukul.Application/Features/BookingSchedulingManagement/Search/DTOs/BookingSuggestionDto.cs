namespace SportsGurukul.Application.Features.BookingSchedulingManagement.Search.DTOs;

public class BookingSuggestionDto
{
    public string Text { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public Guid? RelatedId { get; set; }
    public string? Highlight { get; set; }
}
