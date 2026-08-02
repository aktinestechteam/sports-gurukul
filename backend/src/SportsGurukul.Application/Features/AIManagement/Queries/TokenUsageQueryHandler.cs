using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class TokenUsageQueryHandler
    : IRequestHandler<TokenUsageQuery, Result<PaginatedResult<TokenUsageSummaryDto>>>
{
    private readonly IAITokenUsageRepository _tokenUsageRepo;

    public TokenUsageQueryHandler(IAITokenUsageRepository tokenUsageRepo)
    {
        _tokenUsageRepo = tokenUsageRepo;
    }

    public async Task<Result<PaginatedResult<TokenUsageSummaryDto>>> Handle(TokenUsageQuery request, CancellationToken cancellationToken)
    {
        var query = await _tokenUsageRepo.FindAsync(t => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (request.ConversationId.HasValue)
            filtered = filtered.Where(t => t.ConversationId == request.ConversationId.Value);

        if (!string.IsNullOrWhiteSpace(request.UserId))
            filtered = filtered.Where(t => t.UserId != null && t.UserId.Equals(request.UserId, StringComparison.OrdinalIgnoreCase));

        if (request.FromDate.HasValue)
            filtered = filtered.Where(t => t.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            filtered = filtered.Where(t => t.CreatedAt <= request.ToDate.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TokenUsageSummaryDto(
                t.Id, t.ModelName, t.TotalTokens, t.Cost, t.RequestType, t.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<TokenUsageSummaryDto>>.Success(
            new PaginatedResult<TokenUsageSummaryDto>(paged, total, request.Page, request.PageSize));
    }
}
