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

public class KnowledgeService : IKnowledgeService
{
    private readonly IKnowledgeBaseRepository _baseRepository;
    private readonly IKnowledgeDocumentRepository _documentRepository;
    private readonly IKnowledgeSourceRepository _sourceRepository;
    private readonly IPublisher _publisher;
    private readonly ILogger<KnowledgeService> _logger;

    public KnowledgeService(
        IKnowledgeBaseRepository baseRepository,
        IKnowledgeDocumentRepository documentRepository,
        IKnowledgeSourceRepository sourceRepository,
        IPublisher publisher,
        ILogger<KnowledgeService> logger)
    {
        _baseRepository = baseRepository;
        _documentRepository = documentRepository;
        _sourceRepository = sourceRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result<KnowledgeBase>> CreateBaseAsync(CreateKnowledgeBaseRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new KnowledgeBase
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Visibility = request.Visibility,
            Category = request.Category,
            Tags = request.Tags,
            Status = KnowledgeBaseStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        await _baseRepository.AddAsync(entity, cancellationToken);

        _logger.LogInformation("Created knowledge base {KnowledgeBaseId} with name {Name}", entity.Id, entity.Name);

        return Result<KnowledgeBase>.Success(entity);
    }

    public async Task<Result<KnowledgeBase>> UpdateBaseAsync(UpdateKnowledgeBaseRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _baseRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<KnowledgeBase>.Failure("Knowledge base not found");

        if (request.Name is not null) entity.Name = request.Name;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.Visibility.HasValue) entity.Visibility = request.Visibility.Value;
        if (request.Category is not null) entity.Category = request.Category;
        if (request.Tags is not null) entity.Tags = request.Tags;
        entity.UpdatedAt = DateTime.UtcNow;

        _baseRepository.Update(entity);

        await _publisher.Publish(new KnowledgeBaseUpdatedEvent(entity.Id, entity.Name, DateTime.UtcNow), cancellationToken);

        _logger.LogInformation("Updated knowledge base {KnowledgeBaseId}", request.Id);

        return Result<KnowledgeBase>.Success(entity);
    }

    public async Task<Result<bool>> AttachDocumentAsync(Guid knowledgeBaseId, Guid documentId, CancellationToken cancellationToken = default)
    {
        var kb = await _baseRepository.GetByIdAsync(knowledgeBaseId, cancellationToken);
        if (kb is null || kb.IsDeleted)
            return Result<bool>.Failure("Knowledge base not found");

        var document = await _documentRepository.GetByIdAsync(documentId, cancellationToken);
        if (document is null || document.IsDeleted)
            return Result<bool>.Failure("Document not found");

        kb.TotalDocuments++;
        kb.UpdatedAt = DateTime.UtcNow;

        _baseRepository.Update(kb);

        _logger.LogInformation("Attached document {DocumentId} to knowledge base {KnowledgeBaseId}", documentId, knowledgeBaseId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DetachDocumentAsync(Guid knowledgeBaseId, Guid documentId, CancellationToken cancellationToken = default)
    {
        var kb = await _baseRepository.GetByIdAsync(knowledgeBaseId, cancellationToken);
        if (kb is null || kb.IsDeleted)
            return Result<bool>.Failure("Knowledge base not found");

        var document = await _documentRepository.GetByIdAsync(documentId, cancellationToken);
        if (document is null || document.IsDeleted)
            return Result<bool>.Failure("Document not found");

        kb.TotalDocuments = Math.Max(0, kb.TotalDocuments - 1);
        kb.UpdatedAt = DateTime.UtcNow;

        _baseRepository.Update(kb);

        _logger.LogInformation("Detached document {DocumentId} from knowledge base {KnowledgeBaseId}", documentId, knowledgeBaseId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> RebuildIndexAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        var kb = await _baseRepository.GetByIdAsync(knowledgeBaseId, cancellationToken);
        if (kb is null || kb.IsDeleted)
            return Result<bool>.Failure("Knowledge base not found");

        kb.UpdatedAt = DateTime.UtcNow;
        _baseRepository.Update(kb);

        _logger.LogInformation("Rebuilding index for knowledge base {KnowledgeBaseId}", knowledgeBaseId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<KnowledgeBase>> GetBaseByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _baseRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<KnowledgeBase>.Failure("Knowledge base not found");

        return Result<KnowledgeBase>.Success(entity);
    }

    public async Task<Result<IReadOnlyList<KnowledgeBase>>> SearchBasesAsync(SearchKnowledgeBasesRequest request, CancellationToken cancellationToken = default)
    {
        var query = await _baseRepository.FindAsync(k =>
            !k.IsDeleted &&
            (string.IsNullOrEmpty(request.SearchTerm) || k.Name.Contains(request.SearchTerm) || (k.Description != null && k.Description.Contains(request.SearchTerm))) &&
            (!request.Visibility.HasValue || k.Visibility == request.Visibility) &&
            (!request.Status.HasValue || k.Status == request.Status) &&
            (string.IsNullOrEmpty(request.Category) || k.Category == request.Category), cancellationToken);

        return Result<IReadOnlyList<KnowledgeBase>>.Success(query);
    }
}
