using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetConversationHistoryQueryHandler : IRequestHandler<GetConversationHistoryQuery, Result<IReadOnlyList<MessageDto>>>
{
    private readonly IConversationService _conversationService;

    public GetConversationHistoryQueryHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public Task<Result<IReadOnlyList<MessageDto>>> Handle(GetConversationHistoryQuery request, CancellationToken cancellationToken)
        => _conversationService.GetHistoryAsync(request.ConversationId, cancellationToken);
}
