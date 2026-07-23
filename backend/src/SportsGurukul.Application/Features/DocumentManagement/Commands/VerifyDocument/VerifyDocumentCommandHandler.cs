using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.VerifyDocument;

public class VerifyDocumentCommandHandler : IRequestHandler<VerifyDocumentCommand, Result<AthleteDocumentDto>>
{
    private readonly IAthleteDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<VerifyDocumentCommandHandler> _logger;

    public VerifyDocumentCommandHandler(
        IAthleteDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<VerifyDocumentCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<AthleteDocumentDto>> Handle(VerifyDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Verifying document: {DocumentId}", request.DocumentId);

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
            return Result<AthleteDocumentDto>.Failure("Document not found.");

        document.Status = DocumentStatus.Verified;
        document.VerifiedBy = _currentUser.UserId;
        document.VerifiedOn = DateTime.UtcNow;
        document.UpdatedAt = DateTime.UtcNow;
        _documentRepository.Update(document);

        var audit = new DocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            Action = DocumentAuditAction.Verified,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow,
            Details = "Document verified"
        };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document verified: {DocumentId}", document.Id);

        var downloadUrl = _fileStorageService.GetPublicUrl(document.StoragePath);
        return Result<AthleteDocumentDto>.Success(
            UploadAthleteDocumentCommandHandler.MapToDto(document, downloadUrl));
    }
}
