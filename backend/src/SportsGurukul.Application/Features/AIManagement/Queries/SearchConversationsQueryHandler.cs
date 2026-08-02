using MediatR;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;

namespace SportsGurukul.Application.Features.AIManagement.Queries;

public class SearchConversationsQueryHandler
    : IRequestHandler<SearchConversationsQuery, Result<PaginatedResult<ConversationSummaryDto>>>
{
    private readonly IConversationRepository _conversationRepo;

    public SearchConversationsQueryHandler(IConversationRepository conversationRepo)
    {
        _conversationRepo = conversationRepo;
    }

    public async Task<Result<PaginatedResult<ConversationSummaryDto>>> Handle(SearchConversationsQuery request, CancellationToken cancellationToken)
    {
        var query = await _conversationRepo.FindAsync(c => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            filtered = filtered.Where(c =>
                (c.Title != null && c.Title.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)));

        if (request.AssistantId.HasValue)
            filtered = filtered.Where(c => c.AssistantId == request.AssistantId.Value);

        if (request.UserId.HasValue)
            filtered = filtered.Where(c => c.UserId == request.UserId.Value);

        if (request.Status.HasValue)
            filtered = filtered.Where(c => c.Status == request.Status.Value);

        if (request.FromDate.HasValue)
            filtered = filtered.Where(c => c.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            filtered = filtered.Where(c => c.CreatedAt <= request.ToDate.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(c => new ConversationSummaryDto(
                c.Id, c.Title, c.AssistantId, c.Assistant?.Name,
                c.Status, c.MessageCount, c.LastActivityAt, c.CreatedAt
            ))
            .ToList();

        return Result<PaginatedResult<ConversationSummaryDto>>.Success(
            new PaginatedResult<ConversationSummaryDto>(paged, total, request.Page, request.PageSize));
    }
}
