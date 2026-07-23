using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.Commands.UploadProfilePhoto;

public class UploadProfilePhotoCommandHandler : IRequestHandler<UploadProfilePhotoCommand, Result<ProfilePhotoResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IFileRepository _fileRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UploadProfilePhotoCommandHandler> _logger;

    public UploadProfilePhotoCommandHandler(
        IUserRepository userRepository,
        IUserProfileRepository userProfileRepository,
        IFileRepository fileRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ILogger<UploadProfilePhotoCommandHandler> logger)
    {
        _userRepository = userRepository;
        _userProfileRepository = userProfileRepository;
        _fileRepository = fileRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<ProfilePhotoResponse>> Handle(UploadProfilePhotoCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading profile photo for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return Result<ProfilePhotoResponse>.Failure("User not found.");
        }

        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (profile is null || profile.IsDeleted)
        {
            _logger.LogWarning("Profile not found for user: {UserId}", request.UserId);
            return Result<ProfilePhotoResponse>.Failure("Profile not found. Please create a profile first.");
        }

        var existingFile = await _fileRepository.GetByUserIdAndTypeAsync(request.UserId, FileType.ProfilePhoto, cancellationToken);
        if (existingFile is not null)
        {
            await _fileStorageService.DeleteAsync(existingFile.StoragePath, cancellationToken);
            _fileRepository.Remove(existingFile);
        }

        using var stream = new MemoryStream(request.FileContent);
        var storageResult = await _fileStorageService.UploadAsync(
            stream, request.FileName, request.ContentType, FileCategory.Image, cancellationToken);

        var userFile = new UserFile
        {
            UserId = request.UserId,
            OriginalFileName = request.FileName,
            StoredFileName = storageResult.StoredFileName,
            ContentType = request.ContentType,
            FileSize = storageResult.FileSize,
            StoragePath = storageResult.StoragePath,
            PublicUrl = storageResult.PublicUrl,
            FileType = FileType.ProfilePhoto,
            FileCategory = FileCategory.Image
        };

        await _fileRepository.AddAsync(userFile, cancellationToken);

        profile.ProfileImageUrl = storageResult.PublicUrl ?? storageResult.StoragePath;
        profile.UpdatedAt = DateTime.UtcNow;
        _userProfileRepository.Update(profile);

        user.ProfileImageUrl = storageResult.PublicUrl ?? storageResult.StoragePath;
        user.UpdatedAt = DateTime.UtcNow;
        _userRepository.Update(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Profile photo uploaded for user: {UserId}, FileId: {FileId}", request.UserId, userFile.Id);

        return Result<ProfilePhotoResponse>.Success(new ProfilePhotoResponse
        {
            FileId = userFile.Id,
            Url = storageResult.PublicUrl ?? storageResult.StoragePath,
            FileName = request.FileName,
            FileSize = storageResult.FileSize,
            ContentType = request.ContentType,
            UploadedAt = userFile.CreatedAt
        });
    }
}
