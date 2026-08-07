using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class RegenerateResponseCommandHandler : IRequestHandler<RegenerateResponseCommand, Result<MessageDto>>
{
    private readonly IConversationService _conversationService;

    public RegenerateResponseCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public Task<Result<MessageDto>> Handle(RegenerateResponseCommand request, CancellationToken cancellationToken)
        => _conversationService.RegenerateResponseAsync(request.ConversationId, cancellationToken);
}
