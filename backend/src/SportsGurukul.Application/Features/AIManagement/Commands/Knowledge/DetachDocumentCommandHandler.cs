using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Knowledge;

public class DetachDocumentCommandHandler : IRequestHandler<DetachDocumentCommand, Result<KnowledgeBaseDto>>
{
    private readonly IKnowledgeService _knowledgeService;
    private readonly IUnitOfWork _unitOfWork;

    public DetachDocumentCommandHandler(IKnowledgeService knowledgeService, IUnitOfWork unitOfWork)
    {
        _knowledgeService = knowledgeService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<KnowledgeBaseDto>> Handle(DetachDocumentCommand request, CancellationToken cancellationToken)
    {
        var result = await _knowledgeService.DetachDocumentAsync(request.KnowledgeBaseId, request.DocumentId, cancellationToken);
        if (!result.IsSuccess)
            return Result<KnowledgeBaseDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<KnowledgeBaseDto>.Success(default!);
    }
}
