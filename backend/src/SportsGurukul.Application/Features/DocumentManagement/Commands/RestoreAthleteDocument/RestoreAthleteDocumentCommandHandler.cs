using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.RestoreAthleteDocument;

public class RestoreAthleteDocumentCommandHandler : IRequestHandler<RestoreAthleteDocumentCommand, Result<AthleteDocumentDto>>
{
    private readonly IAthleteDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<RestoreAthleteDocumentCommandHandler> _logger;

    public RestoreAthleteDocumentCommandHandler(
        IAthleteDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<RestoreAthleteDocumentCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<AthleteDocumentDto>> Handle(RestoreAthleteDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Restoring document: {DocumentId}", request.DocumentId);

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is not null)
            return Result<AthleteDocumentDto>.Failure("Document is not deleted.");

        var deletedDoc = await _documentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);
        if (deletedDoc is null)
            return Result<AthleteDocumentDto>.Failure("Document not found.");

        deletedDoc.IsDeleted = false;
        deletedDoc.UpdatedAt = DateTime.UtcNow;
        _documentRepository.Update(deletedDoc);

        var audit = new DocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = deletedDoc.Id,
            Action = DocumentAuditAction.Restored,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow,
            Details = "Document restored"
        };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document restored: {DocumentId}", request.DocumentId);

        var downloadUrl = _fileStorageService.GetPublicUrl(deletedDoc.StoragePath);
        return Result<AthleteDocumentDto>.Success(
            UploadAthleteDocumentCommandHandler.MapToDto(deletedDoc, downloadUrl));
    }
}
