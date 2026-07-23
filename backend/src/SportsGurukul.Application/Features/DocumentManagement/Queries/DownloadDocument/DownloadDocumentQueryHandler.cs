using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.DocumentManagement.Queries.DownloadDocument;

public class DownloadDocumentQueryHandler : IRequestHandler<DownloadDocumentQuery, Result<DocumentDownloadDto>>
{
    private readonly IAthleteDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<DownloadDocumentQueryHandler> _logger;

    public DownloadDocumentQueryHandler(
        IAthleteDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<DownloadDocumentQueryHandler> logger)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<DocumentDownloadDto>> Handle(DownloadDocumentQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading document: {DocumentId}", request.DocumentId);

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
            return Result<DocumentDownloadDto>.Failure("Document not found.");

        var stream = await _fileStorageService.GetAsync(document.StoragePath, cancellationToken);
        if (stream is null)
            return Result<DocumentDownloadDto>.Failure("File not found in storage.");

        var audit = new DocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            Action = DocumentAuditAction.Downloaded,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow
        };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document downloaded: {DocumentId}", document.Id);

        return Result<DocumentDownloadDto>.Success(new DocumentDownloadDto
        {
            DocumentId = document.Id,
            FileName = document.OriginalFileName,
            ContentType = document.MimeType,
            FileSize = document.FileSize,
            Content = stream
        });
    }
}
