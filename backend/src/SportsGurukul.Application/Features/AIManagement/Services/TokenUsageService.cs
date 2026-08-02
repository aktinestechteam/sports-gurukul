using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DomainEvents;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class TokenUsageService : ITokenUsageService
{
    private readonly IAITokenUsageRepository _tokenUsageRepository;
    private readonly IPublisher _publisher;
    private readonly ILogger<TokenUsageService> _logger;

    public TokenUsageService(
        IAITokenUsageRepository tokenUsageRepository,
        IPublisher publisher,
        ILogger<TokenUsageService> logger)
    {
        _tokenUsageRepository = tokenUsageRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result<AITokenUsage>> RecordUsageAsync(RecordTokenUsageRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new AITokenUsage
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            MessageId = request.MessageId,
            ModelName = request.ModelName,
            ProviderName = request.ProviderName,
            PromptTokens = request.PromptTokens,
            CompletionTokens = request.CompletionTokens,
            TotalTokens = request.TotalTokens,
            Cost = request.Cost,
            UserId = request.UserId,
            SessionId = request.SessionId,
            RequestType = request.RequestType,
            CreatedAt = DateTime.UtcNow
        };

        await _tokenUsageRepository.AddAsync(entity, cancellationToken);

        await _publisher.Publish(new TokenUsageRecordedEvent(
            entity.Id, entity.ConversationId, entity.ModelName, entity.TotalTokens, entity.Cost, entity.CreatedAt), cancellationToken);

        _logger.LogInformation("Recorded token usage {TokenUsageId} for model {ModelName}", entity.Id, entity.ModelName);

        return Result<AITokenUsage>.Success(entity);
    }

    public async Task<Result<IReadOnlyList<AITokenUsage>>> GetByConversationAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var result = await _tokenUsageRepository.GetByConversationIdAsync(conversationId, cancellationToken);

        return Result<IReadOnlyList<AITokenUsage>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<AITokenUsage>>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        var result = await _tokenUsageRepository.FindAsync(t => t.UserId == userId, cancellationToken);

        return Result<IReadOnlyList<AITokenUsage>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<AITokenUsage>>> GetByDateRangeAsync(DateTime from, DateTime to, CancellationToken cancellationToken = default)
    {
        var result = await _tokenUsageRepository.GetByDateRangeAsync(from, to, cancellationToken);

        return Result<IReadOnlyList<AITokenUsage>>.Success(result);
    }

    public async Task<Result<IReadOnlyList<AITokenUsage>>> SearchAsync(SearchTokenUsageRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _tokenUsageRepository.FindAsync(t =>
            (string.IsNullOrEmpty(request.ModelName) || t.ModelName.Contains(request.ModelName)) &&
            (string.IsNullOrEmpty(request.UserId) || t.UserId == request.UserId) &&
            (!request.FromDate.HasValue || t.CreatedAt >= request.FromDate.Value) &&
            (!request.ToDate.HasValue || t.CreatedAt <= request.ToDate.Value), cancellationToken);

        return Result<IReadOnlyList<AITokenUsage>>.Success(result);
    }
}
