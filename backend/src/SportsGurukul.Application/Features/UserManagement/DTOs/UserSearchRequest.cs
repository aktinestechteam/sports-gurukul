using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.DTOs;

public class UserSearchRequest : PaginationRequest
{
    public string? SearchTerm { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Mobile { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public RoleType? Role { get; set; }
    public UserStatus? Status { get; set; }
    public Gender? Gender { get; set; }
    public bool? EmailVerified { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsDeleted { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public DateTime? UpdatedFrom { get; set; }
    public DateTime? UpdatedTo { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}
