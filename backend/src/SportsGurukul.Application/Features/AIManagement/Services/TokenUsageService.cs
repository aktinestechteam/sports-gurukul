using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Events;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class TokenUsageService : ITokenUsageService
{
    private readonly ITokenUsageRepository _usageRepository;
    private readonly IConversationRepository _conversationRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<TokenUsageService> _logger;

    public TokenUsageService(
        ITokenUsageRepository usageRepository,
        IConversationRepository conversationRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<TokenUsageService> logger)
    {
        _usageRepository = usageRepository;
        _conversationRepository = conversationRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<TokenUsageDto>> RecordAsync(RecordTokenUsageRequest request, CancellationToken cancellationToken = default)
    {
        var usage = new AITokenUsage
        {
            ProviderId = request.ProviderId,
            ModelId = request.ModelId,
            AssistantId = request.AssistantId,
            ConversationId = request.ConversationId,
            UserId = request.UserId,
            UserType = request.UserType,
            UsageType = request.UsageType,
            InputTokens = request.InputTokens,
            OutputTokens = request.OutputTokens,
            TotalTokens = request.InputTokens + request.OutputTokens,
            CacheReadTokens = request.CacheReadTokens,
            CacheWriteTokens = request.CacheWriteTokens,
            Cost = request.Cost,
            Currency = request.Currency ?? "USD",
            StartedAt = request.StartedAt,
            EndedAt = request.EndedAt,
            LatencyMs = request.LatencyMs,
            ModelName = request.ModelName,
        };

        await _usageRepository.AddAsync(usage, cancellationToken);

        if (request.ConversationId.HasValue)
        {
            var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId.Value, cancellationToken);
            if (conversation is not null)
            {
                conversation.TokenCount += usage.TotalTokens;
                _conversationRepository.Update(conversation);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new TokenUsageRecordedEvent(
                usage.Id,
                request.AssistantId,
                request.ConversationId,
                request.ModelId,
                usage.TotalTokens,
                usage.Cost,
                DateTime.UtcNow),
            cancellationToken);

        _logger.LogInformation(
            "Recorded AI token usage: {TotalTokens} tokens ({UsageType}) for conversation {ConversationId}",
            usage.TotalTokens,
            usage.UsageType,
            request.ConversationId);

        return Result<TokenUsageDto>.Success(MapToDto(usage));
    }

    public async Task<Result<IReadOnlyList<TokenUsageDto>>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var usages = await _usageRepository.GetByConversationAsync(conversationId, cancellationToken);
        return Result<IReadOnlyList<TokenUsageDto>>.Success(usages.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<TokenUsageDto>>> GetByAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        var usages = await _usageRepository.GetByAssistantAsync(assistantId, cancellationToken);
        return Result<IReadOnlyList<TokenUsageDto>>.Success(usages.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<TokenUsageDto>>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var usages = await _usageRepository.GetByUserAsync(userId, cancellationToken);
        return Result<IReadOnlyList<TokenUsageDto>>.Success(usages.Select(MapToDto).ToList());
    }

    public async Task<Result<IReadOnlyList<TokenUsageDto>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var usages = await _usageRepository.GetByDateRangeAsync(from, to, cancellationToken);
        return Result<IReadOnlyList<TokenUsageDto>>.Success(usages.Select(MapToDto).ToList());
    }

    public async Task<Result<TokenUsageSummaryDto>> GetSummaryAsync(
        Guid? assistantId,
        Guid? conversationId,
        Guid? userId,
        DateTime? from,
        DateTime? to,
        AIUsageType? usageType,
        CancellationToken cancellationToken = default)
    {
        var usages = await _usageRepository.FindAsync(
            u =>
                (!assistantId.HasValue || u.AssistantId == assistantId.Value) &&
                (!conversationId.HasValue || u.ConversationId == conversationId.Value) &&
                (!userId.HasValue || u.UserId == userId.Value) &&
                (!from.HasValue || u.CreatedAt >= from.Value) &&
                (!to.HasValue || u.CreatedAt <= to.Value) &&
                (!usageType.HasValue || u.UsageType == usageType.Value),
            cancellationToken);

        var summary = new TokenUsageSummaryDto(
            usages.Count,
            usages.Sum(u => u.InputTokens),
            usages.Sum(u => u.OutputTokens),
            usages.Sum(u => u.TotalTokens),
            usages.Sum(u => u.Cost),
            usages.FirstOrDefault()?.Currency ?? "USD",
            from,
            to);

        return Result<TokenUsageSummaryDto>.Success(summary);
    }

    public async Task<Result<IReadOnlyList<TokenUsageDto>>> SearchAsync(
        Guid? assistantId,
        Guid? conversationId,
        Guid? userId,
        AIUsageType? usageType,
        DateTime? from,
        DateTime? to,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var usages = await _usageRepository.FindAsync(
            u =>
                (!assistantId.HasValue || u.AssistantId == assistantId.Value) &&
                (!conversationId.HasValue || u.ConversationId == conversationId.Value) &&
                (!userId.HasValue || u.UserId == userId.Value) &&
                (!usageType.HasValue || u.UsageType == usageType.Value) &&
                (!from.HasValue || u.CreatedAt >= from.Value) &&
                (!to.HasValue || u.CreatedAt <= to.Value),
            cancellationToken);

        var paged = usages
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<IReadOnlyList<TokenUsageDto>>.Success(paged.Select(MapToDto).ToList());
    }

    private static TokenUsageDto MapToDto(AITokenUsage usage)
        => new(
            usage.Id,
            usage.ProviderId,
            usage.ModelId,
            usage.AssistantId,
            usage.ConversationId,
            usage.UserId,
            usage.UserType,
            usage.UsageType,
            usage.InputTokens,
            usage.OutputTokens,
            usage.TotalTokens,
            usage.CacheReadTokens,
            usage.CacheWriteTokens,
            usage.Cost,
            usage.Currency,
            usage.StartedAt,
            usage.EndedAt,
            usage.LatencyMs,
            usage.ModelName,
            usage.CreatedAt);
}
