using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetConversationQueryHandler
    : IRequestHandler<GetConversationQuery, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;

    public GetConversationQueryHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public async Task<Result<ConversationDto>> Handle(GetConversationQuery request, CancellationToken cancellationToken)
    {
        var result = await _conversationService.GetByIdAsync(request.Id, cancellationToken);
        if (!result.IsSuccess)
            return Result<ConversationDto>.Failure(result.Error!);

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
