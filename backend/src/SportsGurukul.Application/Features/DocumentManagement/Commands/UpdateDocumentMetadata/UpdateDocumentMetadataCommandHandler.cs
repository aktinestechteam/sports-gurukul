using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.UpdateDocumentMetadata;

public class UpdateDocumentMetadataCommandHandler : IRequestHandler<UpdateDocumentMetadataCommand, Result<AthleteDocumentDto>>
{
    private readonly IAthleteDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UpdateDocumentMetadataCommandHandler> _logger;

    public UpdateDocumentMetadataCommandHandler(
        IAthleteDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<UpdateDocumentMetadataCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<AthleteDocumentDto>> Handle(UpdateDocumentMetadataCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Updating document metadata: {DocumentId}", request.DocumentId);

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
            return Result<AthleteDocumentDto>.Failure("Document not found.");

        if (request.Title is not null) document.Title = request.Title;
        if (request.Description is not null) document.Description = request.Description;
        if (request.Category.HasValue) document.Category = request.Category.Value;
        if (request.ExpiryDate.HasValue) document.ExpiryDate = request.ExpiryDate.Value;
        if (request.IsPublic.HasValue) document.IsPublic = request.IsPublic.Value;

        _documentRepository.Update(document);

        var audit = new DocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            Action = DocumentAuditAction.Updated,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow,
            Details = "Document metadata updated"
        };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document metadata updated: {DocumentId}", document.Id);

        var downloadUrl = _fileStorageService.GetPublicUrl(document.StoragePath);
        return Result<AthleteDocumentDto>.Success(
            UploadAthleteDocumentCommandHandler.MapToDto(document, downloadUrl));
    }
}
