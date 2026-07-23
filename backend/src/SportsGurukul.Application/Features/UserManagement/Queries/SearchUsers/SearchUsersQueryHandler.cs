using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;

namespace SportsGurukul.Application.Features.UserManagement.Queries.SearchUsers;

public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, Result<UserSearchResponse>>
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

    public async Task<Result<UserSearchResponse>> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Searching users: SearchTerm={SearchTerm}, Page={Page}, PageSize={PageSize}",
            request.SearchTerm, request.Page, request.PageSize);

        var searchRequest = new UserSearchRequest
        {
            SearchTerm = request.SearchTerm,
            Name = request.Name,
            Email = request.Email,
            Mobile = request.Mobile,
            City = request.City,
            State = request.State,
            Country = request.Country,
            Role = request.Role,
            Status = request.Status,
            Gender = request.Gender,
            EmailVerified = request.EmailVerified,
            IsActive = request.IsActive,
            IsDeleted = request.IsDeleted,
            CreatedFrom = request.CreatedFrom,
            CreatedTo = request.CreatedTo,
            UpdatedFrom = request.UpdatedFrom,
            UpdatedTo = request.UpdatedTo,
            SortBy = request.SortBy,
            SortDescending = request.SortDescending,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var (users, totalCount) = await _userProfileRepository.SearchProfilesAsync(searchRequest, cancellationToken);

        var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);

        var response = new UserSearchResponse
        {
            Items = users,
            TotalRecords = totalCount,
            TotalPages = totalPages,
            CurrentPage = request.Page,
            PageSize = request.PageSize
        };

        return Result<UserSearchResponse>.Success(response);
    }
}
