using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.UserManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.UserManagement.Queries.GetProfilePhoto;

public class GetProfilePhotoQueryHandler : IRequestHandler<GetProfilePhotoQuery, Result<ProfilePhotoResponse>>
{
    private readonly IFileRepository _fileRepository;
    private readonly ILogger<GetProfilePhotoQueryHandler> _logger;

    public GetProfilePhotoQueryHandler(
        IFileRepository fileRepository,
        ILogger<GetProfilePhotoQueryHandler> logger)
    {
        _fileRepository = fileRepository;
        _logger = logger;
    }

    public async Task<Result<ProfilePhotoResponse>> Handle(GetProfilePhotoQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting profile photo for user: {UserId}", request.UserId);

        var file = await _fileRepository.GetActiveProfilePhotoAsync(request.UserId, cancellationToken);
        if (file is null)
        {
            _logger.LogWarning("No profile photo found for user: {UserId}", request.UserId);
            return Result<ProfilePhotoResponse>.Failure("No profile photo found.");
        }

        return Result<ProfilePhotoResponse>.Success(new ProfilePhotoResponse
        {
            FileId = file.Id,
            Url = file.PublicUrl ?? file.StoragePath,
            FileName = file.OriginalFileName,
            FileSize = file.FileSize,
            ContentType = file.ContentType,
            UploadedAt = file.CreatedAt
        });
    }
}
