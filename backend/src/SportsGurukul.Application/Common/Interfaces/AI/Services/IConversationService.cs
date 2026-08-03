using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IConversationService
{
    Task<Result<ConversationDto>> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken = default);

    Task<Result<ConversationDto>> RenameAsync(Guid conversationId, string title, CancellationToken cancellationToken = default);

    Task<Result<ConversationDto>> ArchiveAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<MessageDto>> AddMessageAsync(AddMessageRequest request, CancellationToken cancellationToken = default);

    Task<Result<MessageDto>> RegenerateResponseAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<bool>> ClearMemoryAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<ConversationDto>> SummarizeAsync(SummarizeConversationRequest request, CancellationToken cancellationToken = default);

    Task<Result<ConversationDto>> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<MessageDto>>> GetHistoryAsync(Guid conversationId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<ConversationSummaryDto>>> SearchAsync(
        string? searchTerm,
        Guid? assistantId,
        Guid? participantUserId,
        AIConversationStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
