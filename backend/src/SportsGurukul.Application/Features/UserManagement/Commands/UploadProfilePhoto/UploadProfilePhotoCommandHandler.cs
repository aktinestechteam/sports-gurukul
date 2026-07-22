using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.UserManagement.Commands.UploadProfilePhoto;

public class UploadProfilePhotoCommandHandler : IRequestHandler<UploadProfilePhotoCommand, Result<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadProfilePhotoCommandHandler> _logger;

    public UploadProfilePhotoCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IUnitOfWork unitOfWork,
        ILogger<UploadProfilePhotoCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<string>> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading profile photo for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<string>.Failure("User not found.");
        }

        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile is null || profile.IsDeleted)
        {
            _logger.LogWarning("Profile not found for user: {UserId}", request.UserId);
            return Result<string>.Failure("Profile not found. Please create a profile first.");
        }

        profile.ProfileImageUrl = request.ImageUrl;
        profile.UpdatedAt = DateTime.UtcNow;
        _userProfileRepository.Update(profile);

        user.ProfileImageUrl = request.ImageUrl;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile photo updated for user: {UserId}", request.UserId);

        return Result<string>.Success(request.ImageUrl);
    }
}
