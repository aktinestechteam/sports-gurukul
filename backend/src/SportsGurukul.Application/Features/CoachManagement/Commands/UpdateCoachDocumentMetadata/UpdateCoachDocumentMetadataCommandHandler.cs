using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.UpdateCoachDocumentMetadata;

public class UpdateCoachDocumentMetadataCommandHandler : IRequestHandler<UpdateCoachDocumentMetadataCommand, Result<CoachDocumentDto>>
{
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateCoachDocumentMetadataCommandHandler> _logger;

    public UpdateCoachDocumentMetadataCommandHandler(
        ICoachDocumentRepository coachDocumentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<UpdateCoachDocumentMetadataCommandHandler> logger)
    {
        _coachDocumentRepository = coachDocumentRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<CoachDocumentDto>> Handle(UpdateCoachDocumentMetadataCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating metadata for document: {DocumentId}", request.DocumentId);

        var document = await _coachDocumentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            _logger.LogWarning("Document not found: {DocumentId}", request.DocumentId);
            return Result<CoachDocumentDto>.Failure("Document not found.");
        }

        if (document.IsDeleted)
        {
            _logger.LogWarning("Cannot update deleted document: {DocumentId}", request.DocumentId);
            return Result<CoachDocumentDto>.Failure("Cannot update a deleted document.");
        }

        if (request.Title is not null)
            document.Title = request.Title;
        if (request.Description is not null)
            document.Description = request.Description;
        if (request.Category.HasValue)
            document.Category = request.Category.Value;
        if (request.ExpiryDate.HasValue)
            document.ExpiryDate = request.ExpiryDate.Value;
        if (request.Remarks is not null)
            document.Remarks = request.Remarks;
        if (request.IsPublic.HasValue)
            document.IsPublic = request.IsPublic.Value;

        document.UpdatedAt = DateTime.UtcNow;

        _coachDocumentRepository.Update(document);

        var audit = new CoachDocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            Action = DocumentAuditAction.Updated,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow,
            Details = "Metadata updated",
            CreatedAt = DateTime.UtcNow
        };

        await _coachDocumentRepository.AddAuditAsync(audit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document metadata updated: {DocumentId}", request.DocumentId);

        var dto = MapToDto(document);

        return Result<CoachDocumentDto>.Success(dto);
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
            RejectionReason = document.Status == DocumentStatus.Rejected ? document.Remarks : null,
            IsDeleted = document.IsDeleted,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
