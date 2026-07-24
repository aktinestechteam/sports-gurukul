using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.RejectCoachDocument;

public class RejectCoachDocumentCommandHandler : IRequestHandler<RejectCoachDocumentCommand, Result<CoachDocumentDto>>
{
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<RejectCoachDocumentCommandHandler> _logger;

    public RejectCoachDocumentCommandHandler(
        ICoachDocumentRepository coachDocumentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<RejectCoachDocumentCommandHandler> logger)
    {
        _coachDocumentRepository = coachDocumentRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<CoachDocumentDto>> Handle(RejectCoachDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Rejecting document: {DocumentId}", request.DocumentId);

        var document = await _coachDocumentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            _logger.LogWarning("Document not found: {DocumentId}", request.DocumentId);
            return Result<CoachDocumentDto>.Failure("Document not found.");
        }

        var now = DateTime.UtcNow;

        document.Status = DocumentStatus.Rejected;
        document.Remarks = request.Reason;
        document.UpdatedAt = now;

        _coachDocumentRepository.Update(document);

        var audit = new CoachDocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            Action = DocumentAuditAction.Rejected,
            PerformedBy = _currentUser.UserId,
            PerformedOn = now,
            Details = request.Reason,
            CreatedAt = now
        };

        await _coachDocumentRepository.AddAuditAsync(audit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document rejected: {DocumentId}", request.DocumentId);

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
