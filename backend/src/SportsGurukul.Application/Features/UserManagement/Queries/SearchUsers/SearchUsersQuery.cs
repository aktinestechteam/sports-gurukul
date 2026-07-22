using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.Queries.SearchUsers;

public class SearchUsersQuery : IRequest<Result<SearchUserResponse>>
{
    public string? SearchTerm { get; set; }
    public RoleType? Role { get; set; }
    public string? Sport { get; set; }
    public UserStatus? Status { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
