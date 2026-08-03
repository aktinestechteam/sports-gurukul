using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class GetTokenUsageSummaryQueryHandler : IRequestHandler<GetTokenUsageSummaryQuery, Result<TokenUsageSummaryDto>>
{
    private readonly ITokenUsageService _tokenUsageService;

    public GetTokenUsageSummaryQueryHandler(ITokenUsageService tokenUsageService)
    {
        _tokenUsageService = tokenUsageService;
    }

    public Task<Result<TokenUsageSummaryDto>> Handle(GetTokenUsageSummaryQuery request, CancellationToken cancellationToken)
        => _tokenUsageService.GetSummaryAsync(
            request.AssistantId,
            request.ConversationId,
            request.UserId,
            request.From,
            request.To,
            request.UsageType,
            cancellationToken);
}
