using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.UserManagement.Commands.DeleteUserProfile;

public class DeleteUserProfileCommandHandler : IRequestHandler<DeleteUserProfileCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteUserProfileCommandHandler> _logger;

    public DeleteUserProfileCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteUserProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting profile for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<Unit>.Failure("User not found.");
        }

        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile is null || profile.IsDeleted)
        {
            _logger.LogWarning("Profile not found or already deleted for user: {UserId}", request.UserId);
            return Result<Unit>.Failure("Profile not found.");
        }

        _userProfileRepository.Remove(profile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile deleted for user: {UserId}, ProfileId: {ProfileId}", request.UserId, profile.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
