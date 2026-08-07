using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class RenameConversationCommandHandler : IRequestHandler<RenameConversationCommand, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;

    public RenameConversationCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public Task<Result<ConversationDto>> Handle(RenameConversationCommand request, CancellationToken cancellationToken)
        => _conversationService.RenameAsync(request.ConversationId, request.Title, cancellationToken);
}
