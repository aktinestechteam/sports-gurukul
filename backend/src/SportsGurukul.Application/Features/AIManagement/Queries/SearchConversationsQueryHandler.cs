using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchConversationsQueryHandler : IRequestHandler<SearchConversationsQuery, Result<IReadOnlyList<ConversationSummaryDto>>>
{
    private readonly IConversationService _conversationService;

    public SearchConversationsQueryHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public Task<Result<IReadOnlyList<ConversationSummaryDto>>> Handle(SearchConversationsQuery request, CancellationToken cancellationToken)
        => _conversationService.SearchAsync(
            request.SearchTerm,
            request.AssistantId,
            request.ParticipantUserId,
            request.Status,
            request.Page,
            request.PageSize,
            cancellationToken);
}
