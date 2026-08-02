using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IConversationService
{
    Task<Result<Conversation>> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken = default);
    Task<Result<Conversation>> RenameAsync(Guid id, string title, CancellationToken cancellationToken = default);
    Task<Result<Conversation>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<ConversationMessage>> AddMessageAsync(AddMessageRequest request, CancellationToken cancellationToken = default);
    Task<Result<ConversationMessage>> RegenerateResponseAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ClearMemoryAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<Result<string>> SummarizeAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<Result<Conversation>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ConversationMessage>>> GetHistoryAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<Conversation>>> SearchAsync(SearchConversationsRequest request, CancellationToken cancellationToken = default);
}
