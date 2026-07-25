using System.Security.Cryptography;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UploadCoachDocument;

public class UploadCoachDocumentCommandHandler : IRequestHandler<UploadCoachDocumentCommand, Result<CoachDocumentDto>>
{
    private readonly ICoachRepository _coachRepository;
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UploadCoachDocumentCommandHandler> _logger;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".jpg", ".jpeg", ".png", ".webp"
    };

    private static readonly HashSet<string> BlockedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".exe", ".bat", ".cmd", ".com", ".msi", ".scr", ".pif", ".js", ".vbs", ".ps1", ".sh"
    };

    private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB

    public UploadCoachDocumentCommandHandler(
        ICoachRepository coachRepository,
        ICoachDocumentRepository coachDocumentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<UploadCoachDocumentCommandHandler> logger)
    {
        _coachRepository = coachRepository;
        _coachDocumentRepository = coachDocumentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<CoachDocumentDto>> Handle(UploadCoachDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Uploading document for coach: {CoachId}", request.CoachId);

        var coach = await _coachRepository.GetByIdAsync(request.CoachId, cancellationToken);
        if (coach is null)
        {
            _logger.LogWarning("Coach not found: {CoachId}", request.CoachId);
            return Result<CoachDocumentDto>.Failure("Coach not found.");
        }

        var file = request.File;
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (BlockedExtensions.Contains(extension))
        {
            _logger.LogWarning("Blocked file extension: {Extension}", extension);
            return Result<CoachDocumentDto>.Failure($"File extension '{extension}' is not allowed.");
        }

        if (!AllowedExtensions.Contains(extension))
        {
            _logger.LogWarning("Invalid file extension: {Extension}", extension);
            return Result<CoachDocumentDto>.Failure($"File extension '{extension}' is not allowed. Allowed extensions: {string.Join(", ", AllowedExtensions)}.");
        }

        if (file.Length > MaxFileSize)
        {
            _logger.LogWarning("File too large: {FileSize} bytes", file.Length);
            return Result<CoachDocumentDto>.Failure("File size must not exceed 20 MB.");
        }

        await using var memoryStream = new MemoryStream();
        await file.OpenReadStream().CopyToAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;
        var checksum = await ComputeChecksumAsync(memoryStream, cancellationToken);
        memoryStream.Position = 0;

        var sanitizedFileName = Path.GetFileName(file.FileName);
        var storageResult = await _fileStorageService.UploadAsync(
            memoryStream,
            sanitizedFileName,
            file.ContentType,
            FileCategory.Document,
            cancellationToken);

        var documentId = Guid.NewGuid();
        var now = DateTime.UtcNow;

        var document = new CoachDocument
        {
            Id = documentId,
            CoachId = request.CoachId,
            Category = request.Category,
            Title = request.Title,
            Description = request.Description,
            OriginalFileName = file.FileName,
            StoredFileName = storageResult.StoredFileName,
            StorageProvider = "Local",
            StoragePath = storageResult.StoragePath,
            MimeType = file.ContentType,
            Extension = extension,
            FileSize = storageResult.FileSize,
            Checksum = checksum,
            Version = 1,
            Status = DocumentStatus.Pending,
            UploadedBy = _currentUser.UserId,
            UploadedOn = now,
            ExpiryDate = request.ExpiryDate,
            Remarks = request.Remarks,
            IsPublic = request.IsPublic,
            CreatedAt = now
        };

        await _coachDocumentRepository.AddAsync(document, cancellationToken);

        var version = new CoachDocumentVersion
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            VersionNumber = 1,
            StoredFileName = storageResult.StoredFileName,
            StoragePath = storageResult.StoragePath,
            FileSize = storageResult.FileSize,
            Checksum = checksum,
            UploadedBy = _currentUser.UserId,
            CreatedAt = now
        };

        await _coachDocumentRepository.AddVersionAsync(version, cancellationToken);

        var audit = new CoachDocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = documentId,
            Action = DocumentAuditAction.Uploaded,
            PerformedBy = _currentUser.UserId,
            PerformedOn = now,
            Details = $"Document uploaded: {file.FileName}",
            CreatedAt = now
        };

        await _coachDocumentRepository.AddAuditAsync(audit, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document uploaded: {DocumentId} for coach: {CoachId}", documentId, request.CoachId);

        var dto = MapToDto(document);

        return Result<CoachDocumentDto>.Success(dto);
    }

    private static async Task<string?> ComputeChecksumAsync(Stream stream, CancellationToken cancellationToken)
    {
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static CoachDocumentDto MapToDto(CoachDocument document)
    {
        return new CoachDocumentDto
        {
            Id = document.Id,
            CoachId = document.CoachId,
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
            Remarks = document.Remarks,
            IsDeleted = document.IsDeleted,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
