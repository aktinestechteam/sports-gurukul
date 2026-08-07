using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class SummarizeConversationCommandHandler : IRequestHandler<SummarizeConversationCommand, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;

    public SummarizeConversationCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public async Task<Result<ConversationDto>> Handle(SummarizeConversationCommand request, CancellationToken cancellationToken)
    {
        var summarizeRequest = new SummarizeConversationRequest(request.ConversationId, request.Summary);
        return await _conversationService.SummarizeAsync(summarizeRequest, cancellationToken);
    }
}
