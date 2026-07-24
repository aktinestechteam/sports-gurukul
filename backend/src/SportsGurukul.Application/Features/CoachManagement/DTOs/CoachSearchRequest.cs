using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.DTOs;

public class CoachSearchRequest
{
    private const int MaxPageSize = 100;
    private int _pageSize = 20;

    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? SportName { get; set; }
    public string? CertificationName { get; set; }
    public int? MinExperience { get; set; }
    public int? MaxExperience { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? Language { get; set; }
    public bool? OnlineAvailable { get; set; }
    public bool? OfflineAvailable { get; set; }
    public CoachingLevel? CoachingLevel { get; set; }
    public CoachStatus? Status { get; set; }
    public VerificationStatus? VerificationStatus { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
    }
}
