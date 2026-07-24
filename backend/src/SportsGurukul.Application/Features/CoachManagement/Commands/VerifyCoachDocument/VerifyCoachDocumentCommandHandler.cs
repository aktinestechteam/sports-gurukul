using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.VerifyCoachDocument;

public class VerifyCoachDocumentCommandHandler : IRequestHandler<VerifyCoachDocumentCommand, Result<CoachDocumentDto>>
{
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<VerifyCoachDocumentCommandHandler> _logger;

    public VerifyCoachDocumentCommandHandler(
        ICoachDocumentRepository coachDocumentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<VerifyCoachDocumentCommandHandler> logger)
    {
        _coachDocumentRepository = coachDocumentRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<CoachDocumentDto>> Handle(VerifyCoachDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verifying document: {DocumentId}", request.DocumentId);

        var document = await _coachDocumentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            _logger.LogWarning("Document not found: {DocumentId}", request.DocumentId);
            return Result<CoachDocumentDto>.Failure("Document not found.");
        }

        if (document.Status == DocumentStatus.Verified)
        {
            _logger.LogWarning("Document is already verified: {DocumentId}", request.DocumentId);
            return Result<CoachDocumentDto>.Failure("Document is already verified.");
        }

        var now = DateTime.UtcNow;

        document.Status = DocumentStatus.Verified;
        document.VerifiedBy = _currentUser.UserId;
        document.VerifiedOn = now;
        document.UpdatedAt = now;

        _coachDocumentRepository.Update(document);

        var audit = new CoachDocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            Action = DocumentAuditAction.Verified,
            PerformedBy = _currentUser.UserId,
            PerformedOn = now,
            Details = request.Comments,
            CreatedAt = now
        };

        await _coachDocumentRepository.AddAuditAsync(audit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document verified: {DocumentId}", request.DocumentId);

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
