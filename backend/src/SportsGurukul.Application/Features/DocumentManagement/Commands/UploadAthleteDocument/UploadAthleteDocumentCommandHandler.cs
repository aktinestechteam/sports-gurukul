using System.Security.Cryptography;
using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;

public class UploadAthleteDocumentCommandHandler : IRequestHandler<UploadAthleteDocumentCommand, Result<AthleteDocumentDto>>
{
    private readonly IAthleteDocumentRepository _documentRepository;
    private readonly IAthleteRepository _athleteRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UploadAthleteDocumentCommandHandler> _logger;

    public UploadAthleteDocumentCommandHandler(
        IAthleteDocumentRepository documentRepository,
        IAthleteRepository athleteRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<UploadAthleteDocumentCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _athleteRepository = athleteRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<AthleteDocumentDto>> Handle(UploadAthleteDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading document for athlete: {AthleteId}", request.AthleteId);

        var athlete = await _athleteRepository.GetByIdAsync(request.AthleteId, cancellationToken);
        if (athlete is null)
            return Result<AthleteDocumentDto>.Failure("Athlete not found.");

        var extension = Path.GetExtension(request.File.FileName).ToLowerInvariant();
        var category = GetFileCategory(request.Category);

        await using var stream = request.File.OpenReadStream();
        var checksum = await ComputeChecksumAsync(stream, cancellationToken);
        stream.Position = 0;

        var storageResult = await _fileStorageService.UploadAsync(
            stream, request.File.FileName, request.File.ContentType, category, cancellationToken);

        var document = new AthleteDocument
        {
            Id = Guid.NewGuid(),
            AthleteId = request.AthleteId,
            Category = request.Category,
            Title = request.Title,
            Description = request.Description,
            OriginalFileName = request.File.FileName,
            StoredFileName = storageResult.StoredFileName,
            StorageProvider = _fileStorageService.GetType().Name,
            StoragePath = storageResult.StoragePath,
            MimeType = request.File.ContentType,
            Extension = extension,
            FileSize = storageResult.FileSize,
            Checksum = checksum,
            Version = 1,
            Status = DocumentStatus.Pending,
            UploadedBy = _currentUser.UserId,
            UploadedOn = DateTime.UtcNow,
            ExpiryDate = request.ExpiryDate,
            IsPublic = request.IsPublic
        };

        await _documentRepository.AddAsync(document, cancellationToken);

        var version = new DocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            VersionNumber = 1,
            StoredFileName = storageResult.StoredFileName,
            StoragePath = storageResult.StoragePath,
            FileSize = storageResult.FileSize,
            Checksum = checksum,
            UploadedBy = _currentUser.UserId
        };

        var audit = new DocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            Action = DocumentAuditAction.Uploaded,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow,
            Details = $"Document uploaded: {request.File.FileName}"
        };

        await _documentRepository.AddAsync(document, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document uploaded: {DocumentId} for athlete: {AthleteId}", document.Id, request.AthleteId);

        return Result<AthleteDocumentDto>.Success(MapToDto(document, null));
    }

    private static FileCategory GetFileCategory(DocumentCategory category) => category switch
    {
        DocumentCategory.PlayerPhoto or DocumentCategory.PlayerProfileImage => FileCategory.Image,
        _ => FileCategory.Document
    };

    private static async Task<string> ComputeChecksumAsync(Stream stream, CancellationToken cancellationToken)
    {
        var hash = await MD5.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    internal static AthleteDocumentDto MapToDto(AthleteDocument document, string? downloadUrl)
    {
        return new AthleteDocumentDto
        {
            Id = document.Id,
            AthleteId = document.AthleteId,
            Category = document.Category.ToString(),
            Title = document.Title,
            Description = document.Description,
            OriginalFileName = document.OriginalFileName,
            MimeType = document.MimeType,
            Extension = document.Extension,
            FileSize = document.FileSize,
            Checksum = document.Checksum,
            Version = document.Version,
            Status = document.Status.ToString(),
            UploadedBy = document.UploadedBy,
            UploadedOn = document.UploadedOn,
            VerifiedBy = document.VerifiedBy,
            VerifiedOn = document.VerifiedOn,
            ExpiryDate = document.ExpiryDate,
            IsPublic = document.IsPublic,
            DownloadUrl = downloadUrl,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
