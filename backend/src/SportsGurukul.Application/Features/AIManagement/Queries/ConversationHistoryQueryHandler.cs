using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class ConversationHistoryQueryHandler
    : IRequestHandler<ConversationHistoryQuery, Result<PaginatedResult<MessageDto>>>
{
    private readonly IConversationRepository _conversationRepo;
    private readonly IConversationMessageRepository _messageRepo;

    public ConversationHistoryQueryHandler(IConversationRepository conversationRepo, IConversationMessageRepository messageRepo)
    {
        _conversationRepo = conversationRepo;
        _messageRepo = messageRepo;
    }

    public async Task<Result<PaginatedResult<MessageDto>>> Handle(ConversationHistoryQuery request, CancellationToken cancellationToken)
    {
        var conversation = await _conversationRepo.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
            return Result<PaginatedResult<MessageDto>>.Failure($"Conversation {request.ConversationId} not found");

        var messages = await _messageRepo.GetByConversationIdAsync(request.ConversationId, cancellationToken);
        var total = messages.Count;
        var paged = messages
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(m => new MessageDto(
                m.Id, m.ConversationId, m.Role, m.Status, m.Content,
                m.TokensUsed, m.ToolCalls, m.ToolResults, m.ErrorMessage,
                m.Cost, m.LatencyMs, m.Metadata, m.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<MessageDto>>.Success(
            new PaginatedResult<MessageDto>(paged, total, request.Page, request.PageSize));
    }
}
