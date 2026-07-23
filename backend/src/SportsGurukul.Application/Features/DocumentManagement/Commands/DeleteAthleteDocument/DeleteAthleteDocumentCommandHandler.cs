using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.DocumentManagement.Commands.DeleteAthleteDocument;

public class DeleteAthleteDocumentCommandHandler : IRequestHandler<DeleteAthleteDocumentCommand, Result<Unit>>
{
    private readonly IAthleteDocumentRepository _documentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<DeleteAthleteDocumentCommandHandler> _logger;

    public DeleteAthleteDocumentCommandHandler(
        IAthleteDocumentRepository documentRepository,
        IFileStorageService fileStorageService,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<DeleteAthleteDocumentCommandHandler> logger)
    {
        _documentRepository = documentRepository;
        _fileStorageService = fileStorageService;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteAthleteDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting document: {DocumentId}", request.DocumentId);

        var document = await _documentRepository.GetByIdAsync(request.DocumentId, cancellationToken);
        if (document is null)
            return Result<Unit>.Failure("Document not found.");

        await _fileStorageService.DeleteAsync(document.StoragePath, cancellationToken);

        _documentRepository.Remove(document);

        var audit = new DocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = document.Id,
            Action = DocumentAuditAction.Deleted,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow,
            Details = $"Document deleted: {document.OriginalFileName}"
        };

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document deleted: {DocumentId}", document.Id);

        return Result<Unit>.Success(Unit.Value);
    }
}
