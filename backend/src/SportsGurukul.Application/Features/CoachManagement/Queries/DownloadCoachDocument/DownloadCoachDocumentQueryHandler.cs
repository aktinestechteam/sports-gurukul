using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.CoachManagement.DTOs;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Queries.DownloadCoachDocument;

public class DownloadCoachDocumentQueryHandler : IRequestHandler<DownloadCoachDocumentQuery, Result<CoachDocumentDownloadDto>>
{
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DownloadCoachDocumentQueryHandler> _logger;

    public DownloadCoachDocumentQueryHandler(
        ICoachDocumentRepository coachDocumentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ILogger<DownloadCoachDocumentQueryHandler> logger)
    {
        _coachDocumentRepository = coachDocumentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<CoachDocumentDownloadDto>> Handle(DownloadCoachDocumentQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Downloading coach document: {DocumentId}", request.DocumentId);

        var document = await _coachDocumentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);

        if (document is null)
        {
            return Result<CoachDocumentDownloadDto>.Failure("Document not found.");
        }

        var stream = await _fileStorageService.GetAsync(document.StoragePath, cancellationToken);

        if (stream is null)
        {
            return Result<CoachDocumentDownloadDto>.Failure("File content not found in storage.");
        }

        var audit = new CoachDocumentAudit
        {
            DocumentId = document.Id,
            Action = DocumentAuditAction.Downloaded,
            PerformedOn = DateTime.UtcNow,
            Details = $"Document '{document.Title}' downloaded."
        };

        await _coachDocumentRepository.AddAuditAsync(audit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var dto = new CoachDocumentDownloadDto
        {
            DocumentId = document.Id,
            FileName = document.OriginalFileName,
            ContentType = document.MimeType,
            FileSize = document.FileSize,
            Content = stream
        };

        return Result<CoachDocumentDownloadDto>.Success(dto);
    }
}
