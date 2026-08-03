using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Commands.TokenUsage;

public class RecordTokenUsageCommandHandler : IRequestHandler<RecordTokenUsageCommand, Result<TokenUsageDto>>
{
    private readonly ITokenUsageService _tokenUsageService;

    public RecordTokenUsageCommandHandler(ITokenUsageService tokenUsageService)
    {
        _tokenUsageService = tokenUsageService;
    }

    public async Task<Result<TokenUsageDto>> Handle(RecordTokenUsageCommand request, CancellationToken cancellationToken)
    {
        var usageRequest = new RecordTokenUsageRequest(
            request.ProviderId,
            request.ModelId,
            request.AssistantId,
            request.ConversationId,
            request.UserId,
            request.UserType,
            request.UsageType,
            request.InputTokens,
            request.OutputTokens,
            request.CacheReadTokens,
            request.CacheWriteTokens,
            request.Cost,
            request.Currency,
            request.StartedAt,
            request.EndedAt,
            request.LatencyMs,
            request.ModelName);

        return await _tokenUsageService.RecordAsync(usageRequest, cancellationToken);
    }
}
