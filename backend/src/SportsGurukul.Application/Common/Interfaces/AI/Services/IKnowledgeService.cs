using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IKnowledgeService
{
    Task<Result<KnowledgeBase>> CreateBaseAsync(CreateKnowledgeBaseRequest request, CancellationToken cancellationToken = default);
    Task<Result<KnowledgeBase>> UpdateBaseAsync(UpdateKnowledgeBaseRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> AttachDocumentAsync(Guid knowledgeBaseId, Guid documentId, CancellationToken cancellationToken = default);
    Task<Result<bool>> DetachDocumentAsync(Guid knowledgeBaseId, Guid documentId, CancellationToken cancellationToken = default);
    Task<Result<bool>> RebuildIndexAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default);
    Task<Result<KnowledgeBase>> GetBaseByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyList<KnowledgeBase>>> SearchBasesAsync(SearchKnowledgeBasesRequest request, CancellationToken cancellationToken = default);
}
