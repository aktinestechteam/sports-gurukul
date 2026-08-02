using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class RenameConversationCommandHandler : IRequestHandler<RenameConversationCommand, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;
    private readonly IUnitOfWork _unitOfWork;

    public RenameConversationCommandHandler(IConversationService conversationService, IUnitOfWork unitOfWork)
    {
        _conversationService = conversationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ConversationDto>> Handle(RenameConversationCommand request, CancellationToken cancellationToken)
    {
        var result = await _conversationService.RenameAsync(request.Id, request.Title, cancellationToken);
        if (!result.IsSuccess)
            return Result<ConversationDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var c = result.Value!;
        return Result<ConversationDto>.Success(new ConversationDto(
            c.Id, c.Title, c.AssistantId, c.Assistant?.Name,
            c.UserId, c.Status, c.ContextSummary, c.TokenCount,
            c.MessageCount, c.LastActivityAt, c.Metadata,
            c.CreatedAt, c.UpdatedAt,
            c.Messages?.Select(m => new MessageDto(
                m.Id, m.ConversationId, m.Role, m.Status, m.Content,
                m.TokensUsed, m.ToolCalls, m.ToolResults, m.ErrorMessage,
                m.Cost, m.LatencyMs, m.Metadata, m.CreatedAt
            )).ToList() ?? []
        ));
    }
}
