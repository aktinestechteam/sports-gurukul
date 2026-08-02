using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IAssistantService
{
    Task<Result<AIAssistant>> CreateAsync(CreateAssistantRequest request, CancellationToken cancellationToken = default);
    Task<Result<AIAssistant>> UpdateAsync(UpdateAssistantRequest request, CancellationToken cancellationToken = default);
    Task<Result<AIAssistant>> PublishAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<AIAssistant>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignKnowledgeBaseAsync(Guid assistantId, Guid knowledgeBaseId, CancellationToken cancellationToken = default);
    Task<Result<bool>> AssignToolsAsync(Guid assistantId, List<Guid> toolIds, CancellationToken cancellationToken = default);
    Task<Result<AIAssistant>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<AIAssistant>>> SearchAsync(SearchAssistantsRequest request, CancellationToken cancellationToken = default);
}
