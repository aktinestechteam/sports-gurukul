using MediatR;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.Conversation;

public class CreateConversationCommandHandler : IRequestHandler<CreateConversationCommand, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateConversationCommandHandler(IConversationService conversationService, IUnitOfWork unitOfWork)
    {
        _conversationService = conversationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<ConversationDto>> Handle(CreateConversationCommand request, CancellationToken cancellationToken)
    {
        var createRequest = new CreateConversationRequest(request.Title, request.AssistantId, request.UserId);
        var result = await _conversationService.CreateAsync(createRequest, cancellationToken);
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
