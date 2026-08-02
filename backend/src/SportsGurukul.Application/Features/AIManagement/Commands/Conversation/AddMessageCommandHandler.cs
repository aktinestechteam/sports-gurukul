using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;
    private readonly IUnitOfWork _unitOfWork;

    public AddMessageCommandHandler(IConversationService conversationService, IUnitOfWork unitOfWork)
    {
        _conversationService = conversationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ConversationDto>> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        var addRequest = new AddMessageRequest(request.ConversationId, request.Role, request.Content, request.Metadata);
        var result = await _conversationService.AddMessageAsync(addRequest, cancellationToken);
        if (!result.IsSuccess)
            return Result<ConversationDto>.Failure(result.Error!);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var m = result.Value!;
        return Result<ConversationDto>.Success(new ConversationDto(
            m.Conversation.Id, m.Conversation.Title, m.Conversation.AssistantId, m.Conversation.Assistant?.Name,
            m.Conversation.UserId, m.Conversation.Status, m.Conversation.ContextSummary, m.Conversation.TokenCount,
            m.Conversation.MessageCount, m.Conversation.LastActivityAt, m.Conversation.Metadata,
            m.Conversation.CreatedAt, m.Conversation.UpdatedAt,
            m.Conversation.Messages?.Select(msg => new MessageDto(
                msg.Id, msg.ConversationId, msg.Role, msg.Status, msg.Content,
                msg.TokensUsed, msg.ToolCalls, msg.ToolResults, msg.ErrorMessage,
                msg.Cost, msg.LatencyMs, msg.Metadata, msg.CreatedAt
            )).ToList() ?? []
        ));
    }
}
