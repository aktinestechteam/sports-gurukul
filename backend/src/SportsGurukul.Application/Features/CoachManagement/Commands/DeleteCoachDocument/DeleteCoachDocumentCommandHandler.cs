using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoachDocument;

public class DeleteCoachDocumentCommandHandler : IRequestHandler<DeleteCoachDocumentCommand, Result<Unit>>
{
    private readonly ICoachDocumentRepository _coachDocumentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<DeleteCoachDocumentCommandHandler> _logger;

    public DeleteCoachDocumentCommandHandler(
        ICoachDocumentRepository coachDocumentRepository,
        IUnitOfWork unitOfWork,
        ICurrentUser currentUser,
        ILogger<DeleteCoachDocumentCommandHandler> logger)
    {
        _coachDocumentRepository = coachDocumentRepository;
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(DeleteCoachDocumentCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting document: {DocumentId}", request.DocumentId);

        var document = await _coachDocumentRepository.GetByIdWithDetailsAsync(request.DocumentId, cancellationToken);
        if (document is null)
        {
            _logger.LogWarning("Document not found: {DocumentId}", request.DocumentId);
            return Result<Unit>.Failure("Document not found.");
        }

        if (document.IsDeleted)
        {
            _logger.LogWarning("Document is already deleted: {DocumentId}", request.DocumentId);
            return Result<Unit>.Failure("Document is already deleted.");
        }

        document.IsDeleted = true;
        document.UpdatedAt = DateTime.UtcNow;

        _coachDocumentRepository.Update(document);

        var audit = new CoachDocumentAudit
        {
            Id = Guid.NewGuid(),
            DocumentId = request.DocumentId,
            Action = DocumentAuditAction.Deleted,
            PerformedBy = _currentUser.UserId,
            PerformedOn = DateTime.UtcNow,
            Details = "Document deleted",
            CreatedAt = DateTime.UtcNow
        };

        await _coachDocumentRepository.AddAuditAsync(audit, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Document deleted: {DocumentId}", request.DocumentId);

        return Result<Unit>.Success(Unit.Value);
    }
}
