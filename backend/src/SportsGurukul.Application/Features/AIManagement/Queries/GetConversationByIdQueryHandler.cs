using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetConversationByIdQueryHandler : IRequestHandler<GetConversationByIdQuery, Result<ConversationDto>>
{
    private readonly IConversationService _conversationService;

    public GetConversationByIdQueryHandler(IConversationService conversationService)
    {
        _conversationService = conversationService;
    }

    public Task<Result<ConversationDto>> Handle(GetConversationByIdQuery request, CancellationToken cancellationToken)
        => _conversationService.GetByIdAsync(request.ConversationId, cancellationToken);
}
