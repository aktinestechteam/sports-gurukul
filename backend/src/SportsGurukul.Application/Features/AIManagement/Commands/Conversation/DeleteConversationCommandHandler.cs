using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class DeleteConversationCommandHandler : IRequestHandler<DeleteConversationCommand, Result<bool>>
{
    private readonly IConversationService _conversationService;

    public DeleteConversationCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public Task<Result<bool>> Handle(DeleteConversationCommand request, CancellationToken cancellationToken)
        => _conversationService.DeleteAsync(request.ConversationId, cancellationToken);
}
