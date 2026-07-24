using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachDocumentById;

public class GetCoachDocumentByIdQueryHandler : IRequestHandler<GetCoachDocumentByIdQuery, Result<CoachDocumentDto>>
{
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetCoachDocumentByIdQueryHandler> _logger;

    public GetCoachDocumentByIdQueryHandler(
        ICoachDocumentRepository coachDocumentRepository,
        IFileStorageService fileStorageService,
        ILogger<GetCoachDocumentByIdQueryHandler> logger)
    {
        _coachDocumentRepository = coachDocumentRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<Result<CoachDocumentDto>> Handle(GetCoachDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching coach document: {DocumentId}", request.DocumentId);

        var document = await _coachDocumentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);

        if (document is null)
        {
            return Result<CoachDocumentDto>.Failure("Document not found.");
        }

        var dto = new CoachDocumentDto
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
            RejectionReason = document.Status == DocumentStatus.Rejected ? document.Remarks : null,
            DownloadUrl = _fileStorageService.GetPublicUrl(document.StoragePath),
            IsDeleted = document.IsDeleted,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt,
            Versions = document.Versions.Select(v => new CoachDocumentVersionDto
            {
                Id = v.Id,
                DocumentId = v.DocumentId,
                VersionNumber = v.VersionNumber,
                StoredFileName = v.StoredFileName,
                StoragePath = v.StoragePath,
                FileSize = v.FileSize,
                Checksum = v.Checksum,
                UploadedBy = v.UploadedBy,
                CreatedAt = v.CreatedAt
            }).ToList(),
            AuditTrail = document.AuditTrail.Select(a => new CoachDocumentAuditDto
            {
                Id = a.Id,
                DocumentId = a.DocumentId,
                Action = a.Action.ToString(),
                PerformedBy = a.PerformedBy,
                PerformedOn = a.PerformedOn,
                IpAddress = a.IpAddress,
                Details = a.Details
            }).ToList()
        };

        return Result<CoachDocumentDto>.Success(dto);
    }
}
