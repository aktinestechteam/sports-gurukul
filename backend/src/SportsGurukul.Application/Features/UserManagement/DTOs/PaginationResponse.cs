namespace SportsGurukul.Application.Features.UserManagement.DTOs;

public class PaginationResponse<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
