using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachDocuments;

public class GetCoachDocumentsQueryHandler : IRequestHandler<GetCoachDocumentsQuery, Result<IReadOnlyList<CoachDocumentDto>>>
{
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetCoachDocumentsQueryHandler> _logger;

    public GetCoachDocumentsQueryHandler(
        ICoachDocumentRepository coachDocumentRepository,
        IFileStorageService fileStorageService,
        ILogger<GetCoachDocumentsQueryHandler> logger)
    {
        _coachDocumentRepository = coachDocumentRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<CoachDocumentDto>>> Handle(GetCoachDocumentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching documents for coach: {CoachId}", request.CoachId);

        var documents = await _coachDocumentRepository.GetByCoachIdAsync(request.CoachId, cancellationToken);

        var dtos = documents.Select(d => new CoachDocumentDto
        {
            Id = d.Id,
            CoachId = d.CoachId,
            Category = d.Category.ToString(),
            Title = d.Title,
            Description = d.Description,
            OriginalFileName = d.OriginalFileName,
            MimeType = d.MimeType,
            Extension = d.Extension,
            FileSize = d.FileSize,
            Checksum = d.Checksum,
            Version = d.Version,
            Status = d.Status.ToString(),
            UploadedBy = d.UploadedBy,
            UploadedOn = d.UploadedOn,
            VerifiedBy = d.VerifiedBy,
            VerifiedOn = d.VerifiedOn,
            ExpiryDate = d.ExpiryDate,
            IsPublic = d.IsPublic,
            Remarks = d.Remarks,
            RejectionReason = d.Status == DocumentStatus.Rejected ? d.Remarks : null,
            DownloadUrl = d.IsPublic ? _fileStorageService.GetPublicUrl(d.StoragePath) : null,
            IsDeleted = d.IsDeleted,
            CreatedAt = d.CreatedAt,
            UpdatedAt = d.UpdatedAt,
            Versions = d.Versions.Select(v => new CoachDocumentVersionDto
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
            AuditTrail = d.AuditTrail.Select(a => new CoachDocumentAuditDto
            {
                Id = a.Id,
                DocumentId = a.DocumentId,
                Action = a.Action.ToString(),
                PerformedBy = a.PerformedBy,
                PerformedOn = a.PerformedOn,
                IpAddress = a.IpAddress,
                Details = a.Details
            }).ToList()
        }).ToList();

        return Result<IReadOnlyList<CoachDocumentDto>>.Success(dtos);
    }
}
