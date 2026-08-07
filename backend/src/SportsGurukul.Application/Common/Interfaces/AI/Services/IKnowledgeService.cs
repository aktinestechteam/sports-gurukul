using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Common.Interfaces.AI.Services;

public interface IKnowledgeService
{
    Task<Result<KnowledgeBaseDto>> CreateAsync(CreateKnowledgeBaseRequest request, CancellationToken cancellationToken = default);

    Task<Result<KnowledgeBaseDto>> UpdateAsync(UpdateKnowledgeBaseRequest request, CancellationToken cancellationToken = default);

    Task<Result<KnowledgeDocumentDto>> AttachDocumentAsync(AttachDocumentRequest request, CancellationToken cancellationToken = default);

    Task<Result<bool>> DetachDocumentAsync(Guid knowledgeBaseId, Guid documentId, CancellationToken cancellationToken = default);

    Task<Result<KnowledgeBaseDto>> RebuildIndexAsync(RebuildKnowledgeIndexRequest request, CancellationToken cancellationToken = default);

    Task<Result<KnowledgeBaseDto>> GetByIdAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<KnowledgeDocumentDto>>> GetDocumentsAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default);

    Task<Result<IReadOnlyList<KnowledgeBaseDto>>> SearchAsync(
        string? searchTerm,
        AIKnowledgeBaseType? knowledgeBaseType,
        Guid? ownerUserId,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);
}
