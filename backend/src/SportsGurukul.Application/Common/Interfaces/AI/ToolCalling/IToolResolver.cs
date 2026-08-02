using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.ToolCalling;

public interface IToolResolver
{
    Task<Result<ToolDefinition>> ResolveAsync(string toolName, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ToolDefinition>>> ResolveForConversationAsync(Guid conversationId, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<ToolDefinition>>> ResolveForAssistantAsync(Guid assistantId, CancellationToken cancellationToken = default);
}
