using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;

namespace SportsGurukul.Application.Features.DocumentManagement.Queries.GetDocumentById;

public class GetDocumentByIdQueryHandler : IRequestHandler<GetDocumentByIdQuery, Result<AthleteDocumentDto>>
{
    private readonly IAthleteDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetDocumentByIdQueryHandler> _logger;

    public GetDocumentByIdQueryHandler(
        IAthleteDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        ILogger<GetDocumentByIdQueryHandler> logger)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<Result<AthleteDocumentDto>> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching document: {DocumentId}", request.DocumentId);

        var document = await _documentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);
        if (document is null)
            return Result<AthleteDocumentDto>.Failure("Document not found.");

        var downloadUrl = _fileStorageService.GetPublicUrl(document.StoragePath);
        return Result<AthleteDocumentDto>.Success(
            UploadAthleteDocumentCommandHandler.MapToDto(document, downloadUrl));
    }
}
