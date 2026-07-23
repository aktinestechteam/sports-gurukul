using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.DocumentManagement.Commands.UploadAthleteDocument;
using SportsGurukul.Application.Features.DocumentManagement.DTOs;

namespace SportsGurukul.Application.Features.DocumentManagement.Queries.GetAthleteDocuments;

public class GetAthleteDocumentsQueryHandler : IRequestHandler<GetAthleteDocumentsQuery, Result<IReadOnlyList<AthleteDocumentDto>>>
{
    private readonly IAthleteDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly ILogger<GetAthleteDocumentsQueryHandler> _logger;

    public GetAthleteDocumentsQueryHandler(
        IAthleteDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        ILogger<GetAthleteDocumentsQueryHandler> logger)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _logger = logger;
    }

    public async Task<Result<IReadOnlyList<AthleteDocumentDto>>> Handle(
        GetAthleteDocumentsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching documents for athlete: {AthleteId}", request.AthleteId);

        var documents = await _documentRepository.GetByAthleteIdAsync(request.AthleteId, cancellationToken);

        var dtos = documents.Select(d =>
        {
            var url = _fileStorageService.GetPublicUrl(d.StoragePath);
            return UploadAthleteDocumentCommandHandler.MapToDto(d, url);
        }).ToList();

        return Result<IReadOnlyList<AthleteDocumentDto>>.Success(dtos);
    }
}
