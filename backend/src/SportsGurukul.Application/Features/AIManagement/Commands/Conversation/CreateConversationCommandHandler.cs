using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;

    public CreateConversationCommandHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public async Task<Result<ConversationDto>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreateConversationRequest(
            request.AssistantId,
            request.Title,
            request.ParticipantType,
            request.ParticipantUserId,
            request.KnowledgeBaseIds,
            request.ContextMetadataJson);

        return await _conversationService.CreateAsync(createRequest, cancellationToken);
    }
}
