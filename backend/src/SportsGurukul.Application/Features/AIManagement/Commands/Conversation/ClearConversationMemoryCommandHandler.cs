using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class ClearConversationMemoryCommandHandler : IRequestHandler<ClearConversationMemoryCommand, Result<bool>>
{
    private readonly IConversationService _conversationService;

    public ClearConversationMemoryCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public Task<Result<bool>> Handle(ClearConversationMemoryCommand request, CancellationToken cancellationToken)
        => _conversationService.ClearMemoryAsync(request.ConversationId, cancellationToken);
}
