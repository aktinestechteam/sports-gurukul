namespace SportsGurukul.Application.Features.RegistrationAttendancePlatform.DTOs;

public class PlatformSearchResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
