using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.UserManagement.Commands.RestoreUserProfile;

public class RestoreUserProfileCommandHandler : IRequestHandler<RestoreUserProfileCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<RestoreUserProfileCommandHandler> _logger;

    public RestoreUserProfileCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<RestoreUserProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(RestoreUserProfileCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring profile for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<Unit>.Failure("User not found.");
        }

        var allProfiles = await _userProfileRepository.FindAsync(p => p.UserId == request.UserId, cancellationToken);
        var deletedProfile = allProfiles.FirstOrDefault(p => p.IsDeleted);

        if (deletedProfile is null)
        {
            _logger.LogWarning("No deleted profile found for user: {UserId}", request.UserId);
            return Result<Unit>.Failure("No deleted profile found for this user.");
        }

        deletedProfile.IsDeleted = false;
        deletedProfile.UpdatedAt = DateTime.UtcNow;
        _userProfileRepository.Update(deletedProfile);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile restored for user: {UserId}, ProfileId: {ProfileId}", request.UserId, deletedProfile.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
