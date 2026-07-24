namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachSearchResponse
{
    public IReadOnlyList<CoachSummaryDto> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
