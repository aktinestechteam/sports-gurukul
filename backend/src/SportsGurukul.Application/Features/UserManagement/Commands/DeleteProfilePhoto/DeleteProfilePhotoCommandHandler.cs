using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.Commands.DeleteProfilePhoto;

public class DeleteProfilePhotoCommandHandler : IRequestHandler<DeleteProfilePhotoCommand, Result<Unit>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteProfilePhotoCommandHandler> _logger;

    public DeleteProfilePhotoCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IFileRepository fileRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ILogger<DeleteProfilePhotoCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _fileRepository = fileRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting profile photo for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<Unit>.Failure("User not found.");
        }

        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile is null || profile.IsDeleted)
        {
            _logger.LogWarning("Profile not found for user: {UserId}", request.UserId);
            return Result<Unit>.Failure("Profile not found.");
        }

        var file = await _fileRepository.GetByUserIdAndTypeAsync(request.UserId, FileType.ProfilePhoto, cancellationToken);
        if (file is null)
        {
            _logger.LogWarning("No profile photo found for user: {UserId}", request.UserId);
            return Result<Unit>.Failure("No profile photo to delete.");
        }

        await _fileStorageService.DeleteAsync(file.StoragePath, cancellationToken);
        _fileRepository.Remove(file);

        profile.ProfileImageUrl = null;
        profile.UpdatedAt = DateTime.UtcNow;
        _userProfileRepository.Update(profile);

        user.ProfileImageUrl = null;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile photo deleted for user: {UserId}", request.UserId);

        return Result<Unit>.Success(Unit.Value);
    }
}
