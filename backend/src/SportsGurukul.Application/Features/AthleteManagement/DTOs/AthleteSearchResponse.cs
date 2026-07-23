namespace SportsGurukul.Application.Features.AthleteManagement.DTOs;

public class AthleteSearchResponse
{
    public IReadOnlyList<AthleteSummaryDto> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
    public string? NextCursor { get; set; }
    public string? PreviousCursor { get; set; }
}
