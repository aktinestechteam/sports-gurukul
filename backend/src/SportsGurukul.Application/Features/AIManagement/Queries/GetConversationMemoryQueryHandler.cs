using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetConversationMemoryQueryHandler : IRequestHandler<GetConversationMemoryQuery, Result<IReadOnlyList<ConversationMemoryDto>>>
{
    private readonly IConversationMemoryService _memoryService;

    public GetConversationMemoryQueryHandler(IConversationMemoryService memoryService)
    {
        _memoryService = memoryService;
    }

    public Task<Result<IReadOnlyList<ConversationMemoryDto>>> Handle(GetConversationMemoryQuery request, CancellationToken cancellationToken)
        => _memoryService.GetAsync(request.ConversationId, cancellationToken);
}
