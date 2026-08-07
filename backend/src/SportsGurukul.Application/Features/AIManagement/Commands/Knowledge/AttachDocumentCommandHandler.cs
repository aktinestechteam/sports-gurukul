using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class AttachDocumentCommandHandler : IRequestHandler<AttachDocumentCommand, Result<KnowledgeDocumentDto>>
{
    private readonly IKnowledgeService _knowledgeService;

    public AttachDocumentCommandHandler(IKnowledgeService knowledgeService)
    {
        _knowledgeService = knowledgeService;
    }

    public async Task<Result<KnowledgeDocumentDto>> Handle(AttachDocumentCommand request, CancellationToken cancellationToken)
    {
        var attachRequest = new AttachDocumentRequest(
            request.KnowledgeBaseId,
            request.Title,
            request.DocumentType,
            request.Content,
            request.ExternalId,
            request.StoragePath,
            request.MimeType,
            request.MetadataJson);

        return await _knowledgeService.AttachDocumentAsync(attachRequest, cancellationToken);
    }
}
