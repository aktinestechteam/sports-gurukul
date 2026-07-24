using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoachDocument;

public class RestoreCoachDocumentCommandHandler : IRequestHandler<RestoreCoachDocumentCommand, Result<CoachDocumentDto>>
{
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<RestoreCoachDocumentCommandHandler> _logger;

    public RestoreCoachDocumentCommandHandler(
        ICoachDocumentRepository coachDocumentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<RestoreCoachDocumentCommandHandler> logger)
    {
        _coachDocumentRepository = coachDocumentRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<CoachDocumentDto>> Handle(RestoreCoachDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring document: {DocumentId}", request.DocumentId);

        var document = await _coachDocumentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            _logger.LogWarning("No deleted document found with ID: {DocumentId}", request.DocumentId);
            return Result<CoachDocumentDto>.Failure("No deleted document found with this ID.");
        }

        if (!document.IsDeleted)
        {
            _logger.LogWarning("Document is not deleted: {DocumentId}", request.DocumentId);
            return Result<CoachDocumentDto>.Failure("Document is not deleted.");
        }

        document.IsDeleted = false;
        document.UpdatedAt = DateTime.UtcNow;

        _coachDocumentRepository.Update(document);

        var audit = new CoachDocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            Action = DocumentAuditAction.Restored,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow,
            Details = "Document restored",
            CreatedAt = DateTime.UtcNow
        };

        await _coachDocumentRepository.AddAuditAsync(audit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document restored: {DocumentId}", request.DocumentId);

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
            IsDeleted = document.IsDeleted,
            CreatedAt = document.CreatedAt,
            UpdatedAt = document.UpdatedAt
        };
    }
}
