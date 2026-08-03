using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchTokenUsageQueryHandler : IRequestHandler<SearchTokenUsageQuery, Result<IReadOnlyList<TokenUsageDto>>>
{
    private readonly ITokenUsageService _tokenUsageService;

    public SearchTokenUsageQueryHandler(ITokenUsageService tokenUsageService)
    {
        _tokenUsageService = tokenUsageService;
    }

    public Task<Result<IReadOnlyList<TokenUsageDto>>> Handle(SearchTokenUsageQuery request, CancellationToken cancellationToken)
        => _tokenUsageService.SearchAsync(
            request.AssistantId,
            request.ConversationId,
            request.UserId,
            request.UsageType,
            request.From,
            request.To,
            request.Page,
            request.PageSize,
            cancellationToken);
}
