using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class ArchiveConversationCommandHandler : IRequestHandler<ArchiveConversationCommand, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;

    public ArchiveConversationCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public Task<Result<ConversationDto>> Handle(ArchiveConversationCommand request, CancellationToken cancellationToken)
        => _conversationService.ArchiveAsync(request.ConversationId, cancellationToken);
}
