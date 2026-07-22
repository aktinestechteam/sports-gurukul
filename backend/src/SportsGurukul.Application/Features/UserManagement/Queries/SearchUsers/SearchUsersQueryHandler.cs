using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;

namespace SportsGurukul.Application.Features.UserManagement.Queries.SearchUsers;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, Result<SearchUserResponse>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<SearchUsersQueryHandler> _logger;

    public SearchUsersQueryHandler(
        IUserProfileRepository userProfileRepository,
        ILogger<SearchUsersQueryHandler> logger)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public async Task<Result<SearchUserResponse>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Searching users: SearchTerm={SearchTerm}, Page={Page}, PageSize={PageSize}",
            request.SearchTerm, request.Page, request.PageSize);

        var searchRequest = new SearchUserRequest
        {
            SearchTerm = request.SearchTerm,
            Role = request.Role,
            Sport = request.Sport,
            Status = request.Status,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var (users, totalCount) = await _userProfileRepository.SearchProfilesAsync(searchRequest, cancellationToken);

        var result = new SearchUserResponse
        {
            Users = users,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return Result<SearchUserResponse>.Success(result);
    }
}
