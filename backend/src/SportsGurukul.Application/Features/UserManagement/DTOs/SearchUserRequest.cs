using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.DTOs;

public class SearchUserRequest
{
    public string? SearchTerm { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public RoleType? Role { get; set; }
    public UserStatus? Status { get; set; }
    public string? City { get; set; }
    public string? Sport { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
