using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IConversationMemoryService
{
    Task<Result<bool>> ClearAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<bool>> StoreAsync(
        Guid conversationId,
        AIMemoryType memoryType,
        string key,
        string content,
        int importance,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ConversationMemoryDto>>> GetAsync(Guid conversationId, CancellationToken cancellationToken = default);
}
