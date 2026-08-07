using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.Commands.CreateUserProfile;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Features.UserManagement.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, Result<UserProfileDto>>
{
    private readonly ICurrentUser _currentUser;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        ICurrentUser currentUser,
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _currentUser = currentUser;
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _logger = logger;
    }

    public async Task<Result<UserProfileDto>> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId;
        if (userId is null)
        {
            _logger.LogWarning("No authenticated user found");
            return Result<UserProfileDto>.Failure("User not authenticated.");
        }

        _logger.LogInformation("Getting current user profile: {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId.Value, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            return Result<UserProfileDto>.Failure("User not found.");
        }

        var profile = await _userProfileRepository.GetFullProfileAsync(userId.Value, cancellationToken);
        if (profile is null || profile.IsDeleted)
        {
            _logger.LogInformation(
                "No profile yet for current user: {UserId}; returning identity-only response",
                userId.Value);

            var identityOnly = new UserProfile
            {
                Id = user.Id,
                UserId = user.Id,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.CreatedAt
            };
            var identityDto = CreateUserProfileCommandHandler.MapToDto(identityOnly, user);
            identityDto.HasProfile = false;
            return Result<UserProfileDto>.Success(identityDto);
        }

        var dto = CreateUserProfileCommandHandler.MapToDto(profile, user);
        return Result<UserProfileDto>.Success(dto);
    }
}
