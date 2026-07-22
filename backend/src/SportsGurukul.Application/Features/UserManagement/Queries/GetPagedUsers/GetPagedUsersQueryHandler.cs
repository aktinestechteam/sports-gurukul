using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;

namespace SportsGurukul.Application.Features.UserManagement.Queries.GetPagedUsers;

public class GetPagedUsersQueryHandler : IRequestHandler<GetPagedUsersQuery, Result<SearchUserResponse>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<GetPagedUsersQueryHandler> _logger;

    public GetPagedUsersQueryHandler(
        IUserProfileRepository userProfileRepository,
        ILogger<GetPagedUsersQueryHandler> logger)
    {
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public async Task<Result<SearchUserResponse>> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting paged users: Page={Page}, PageSize={PageSize}", request.Page, request.PageSize);

        var searchRequest = new SearchUserRequest
        {
            Page = request.Page,
            PageSize = request.PageSize,
            SortBy = request.SortBy ?? "CreatedAt",
            SortDescending = request.SortDescending
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
