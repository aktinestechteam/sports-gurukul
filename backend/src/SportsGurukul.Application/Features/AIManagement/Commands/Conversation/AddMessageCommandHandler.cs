using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class AddMessageCommandHandler : IRequestHandler<AddMessageCommand, Result<MessageDto>>
{
    private readonly IConversationService _conversationService;

    public AddMessageCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public async Task<Result<MessageDto>> Handle(AddMessageCommand request, CancellationToken cancellationToken)
    {
        var addRequest = new AddMessageRequest(
            request.ConversationId,
            request.Role,
            request.ContentType,
            request.Content,
            request.ModelName,
            request.PromptVersionUsed,
            request.InputTokenCount,
            request.OutputTokenCount,
            request.LatencyMs,
            request.ToolCallsJson,
            request.ToolResultsJson);

        return await _conversationService.AddMessageAsync(addRequest, cancellationToken);
    }
}
