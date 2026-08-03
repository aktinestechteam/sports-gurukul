using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Events;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class KnowledgeService : IKnowledgeService
{
    private readonly IKnowledgeBaseRepository _knowledgeBaseRepository;
    private readonly IRepository<KnowledgeDocument> _documentRepository;
    private readonly IRepository<AIModel> _modelRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<KnowledgeService> _logger;

    public KnowledgeService(
        IKnowledgeBaseRepository knowledgeBaseRepository,
        IRepository<KnowledgeDocument> documentRepository,
        IRepository<AIModel> modelRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<KnowledgeService> logger)
    {
        _knowledgeBaseRepository = knowledgeBaseRepository;
        _documentRepository = documentRepository;
        _modelRepository = modelRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<KnowledgeBaseDto>> CreateAsync(CreateKnowledgeBaseRequest request, CancellationToken cancellationToken = default)
    {
        if (request.EmbeddingModelId.HasValue)
        {
            var model = await _modelRepository.GetByIdAsync(request.EmbeddingModelId.Value, cancellationToken);
            if (model is null || !model.SupportsEmbeddings)
                return Result<KnowledgeBaseDto>.Failure("The referenced embedding model does not exist or does not support embeddings");
        }

        var knowledgeBase = new KnowledgeBase
        {
            Name = request.Name,
            Description = request.Description,
            KnowledgeBaseType = request.KnowledgeBaseType,
            OwnerType = request.OwnerType,
            OwnerUserId = request.OwnerUserId,
            EmbeddingModelId = request.EmbeddingModelId,
            VectorIndexId = request.VectorIndexId,
            ChunkingStrategy = request.ChunkingStrategy,
            ChunkSize = request.ChunkSize,
            ChunkOverlap = request.ChunkOverlap,
            IsActive = true,
            MetadataSchemaJson = request.MetadataSchemaJson,
            StatisticsJson = "{\"documentCount\":0,\"chunkCount\":0}",
        };

        await _knowledgeBaseRepository.AddAsync(knowledgeBase, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Knowledge base created: {KnowledgeBaseName} ({KnowledgeBaseId})", knowledgeBase.Name, knowledgeBase.Id);
        return Result<KnowledgeBaseDto>.Success(MapToDto(knowledgeBase));
    }

    public async Task<Result<KnowledgeBaseDto>> UpdateAsync(UpdateKnowledgeBaseRequest request, CancellationToken cancellationToken = default)
    {
        var knowledgeBase = await _knowledgeBaseRepository.GetByIdWithDetailsAsync(request.KnowledgeBaseId, cancellationToken);
        if (knowledgeBase is null)
            return Result<KnowledgeBaseDto>.Failure("Knowledge base not found");

        if (request.ExpectedRowVersion is { Length: > 0 } && !knowledgeBase.RowVersion.SequenceEqual(request.ExpectedRowVersion))
            return Result<KnowledgeBaseDto>.Failure("The knowledge base was modified by another user; please refresh and try again");

        if (!string.IsNullOrWhiteSpace(request.Name)) knowledgeBase.Name = request.Name;
        if (request.Description is not null) knowledgeBase.Description = request.Description;
        if (request.KnowledgeBaseType.HasValue) knowledgeBase.KnowledgeBaseType = request.KnowledgeBaseType.Value;
        if (request.EmbeddingModelId.HasValue) knowledgeBase.EmbeddingModelId = request.EmbeddingModelId;
        if (request.VectorIndexId.HasValue) knowledgeBase.VectorIndexId = request.VectorIndexId;
        if (request.ChunkingStrategy.HasValue) knowledgeBase.ChunkingStrategy = request.ChunkingStrategy.Value;
        if (request.ChunkSize.HasValue) knowledgeBase.ChunkSize = request.ChunkSize.Value;
        if (request.ChunkOverlap.HasValue) knowledgeBase.ChunkOverlap = request.ChunkOverlap.Value;
        if (request.MetadataSchemaJson is not null) knowledgeBase.MetadataSchemaJson = request.MetadataSchemaJson;
        if (request.IsActive.HasValue) knowledgeBase.IsActive = request.IsActive.Value;

        knowledgeBase.UpdatedAt = DateTime.UtcNow;
        _knowledgeBaseRepository.Update(knowledgeBase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new KnowledgeBaseUpdatedEvent(knowledgeBase.Id, knowledgeBase.Name, DateTime.UtcNow),
            cancellationToken);

        return Result<KnowledgeBaseDto>.Success(MapToDto(knowledgeBase));
    }

    public async Task<Result<KnowledgeDocumentDto>> AttachDocumentAsync(AttachDocumentRequest request, CancellationToken cancellationToken = default)
    {
        var knowledgeBase = await _knowledgeBaseRepository.GetByIdAsync(request.KnowledgeBaseId, cancellationToken);
        if (knowledgeBase is null)
            return Result<KnowledgeDocumentDto>.Failure("Knowledge base not found");

        var document = new KnowledgeDocument
        {
            KnowledgeBaseId = request.KnowledgeBaseId,
            Title = request.Title,
            DocumentType = request.DocumentType,
            Content = request.Content,
            ContentHash = ComputeHash(request.Content ?? request.Title),
            ExternalId = request.ExternalId,
            StoragePath = request.StoragePath,
            MimeType = request.MimeType,
            Status = AIDocumentStatus.Pending,
            MetadataJson = request.MetadataJson,
        };

        await _documentRepository.AddAsync(document, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<KnowledgeDocumentDto>.Success(MapToDocumentDto(document));
    }

    public async Task<Result<bool>> DetachDocumentAsync(Guid knowledgeBaseId, Guid documentId, CancellationToken cancellationToken = default)
    {
        var knowledgeBase = await _knowledgeBaseRepository.GetByIdWithDetailsAsync(knowledgeBaseId, cancellationToken);
        if (knowledgeBase is null)
            return Result<bool>.Failure("Knowledge base not found");

        var document = knowledgeBase.Documents.FirstOrDefault(d => d.Id == documentId && !d.IsDeleted);
        if (document is null)
            return Result<bool>.Failure("Document not found in the knowledge base");

        document.IsDeleted = true;
        document.UpdatedAt = DateTime.UtcNow;
        _documentRepository.Update(document);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<KnowledgeBaseDto>> RebuildIndexAsync(RebuildKnowledgeIndexRequest request, CancellationToken cancellationToken = default)
    {
        var knowledgeBase = await _knowledgeBaseRepository.GetByIdWithDetailsAsync(request.KnowledgeBaseId, cancellationToken);
        if (knowledgeBase is null)
            return Result<KnowledgeBaseDto>.Failure("Knowledge base not found");

        foreach (var document in knowledgeBase.Documents.Where(d => !d.IsDeleted))
        {
            document.Status = AIDocumentStatus.Pending;
            document.ProcessedAt = null;
            _documentRepository.Update(document);
        }

        knowledgeBase.StatisticsJson = $"{{ \"documentCount\":{knowledgeBase.Documents.Count(d => !d.IsDeleted)}, \"chunkCount\":0 }}";
        knowledgeBase.UpdatedAt = DateTime.UtcNow;
        _knowledgeBaseRepository.Update(knowledgeBase);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Knowledge base {KnowledgeBaseId} queued for re-indexing", knowledgeBase.Id);
        return Result<KnowledgeBaseDto>.Success(MapToDto(knowledgeBase));
    }

    public async Task<Result<KnowledgeBaseDto>> GetByIdAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        var knowledgeBase = await _knowledgeBaseRepository.GetByIdWithDetailsAsync(knowledgeBaseId, cancellationToken);
        if (knowledgeBase is null)
            return Result<KnowledgeBaseDto>.Failure("Knowledge base not found");

        return Result<KnowledgeBaseDto>.Success(MapToDto(knowledgeBase));
    }

    public async Task<Result<IReadOnlyList<KnowledgeDocumentDto>>> GetDocumentsAsync(Guid knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        var knowledgeBase = await _knowledgeBaseRepository.GetByIdWithDetailsAsync(knowledgeBaseId, cancellationToken);
        if (knowledgeBase is null)
            return Result<IReadOnlyList<KnowledgeDocumentDto>>.Failure("Knowledge base not found");

        var documents = knowledgeBase.Documents
            .Where(d => !d.IsDeleted)
            .OrderByDescending(d => d.CreatedAt)
            .Select(MapToDocumentDto)
            .ToList();

        return Result<IReadOnlyList<KnowledgeDocumentDto>>.Success(documents);
    }

    public async Task<Result<IReadOnlyList<KnowledgeBaseDto>>> SearchAsync(
        string? searchTerm,
        AIKnowledgeBaseType? knowledgeBaseType,
        Guid? ownerUserId,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<KnowledgeBase> knowledgeBases;
        if (knowledgeBaseType.HasValue)
            knowledgeBases = await _knowledgeBaseRepository.GetByTypeAsync(knowledgeBaseType.Value, cancellationToken);
        else if (ownerUserId.HasValue)
            knowledgeBases = await _knowledgeBaseRepository.GetByOwnerAsync(ownerUserId.Value, cancellationToken);
        else
            knowledgeBases = await _knowledgeBaseRepository.GetAllAsync(cancellationToken);

        var query = knowledgeBases.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(k =>
                k.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (k.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (isActive.HasValue)
            query = query.Where(k => k.IsActive == isActive.Value);

        var paged = query
            .OrderByDescending(k => k.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<IReadOnlyList<KnowledgeBaseDto>>.Success(paged.Select(MapToDto).ToList());
    }

    private static string ComputeHash(string content)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(content ?? string.Empty));
        return Convert.ToHexString(bytes);
    }

    private static KnowledgeBaseDto MapToDto(KnowledgeBase knowledgeBase)
        => new(
            knowledgeBase.Id,
            knowledgeBase.Name,
            knowledgeBase.Description,
            knowledgeBase.KnowledgeBaseType,
            knowledgeBase.OwnerType,
            knowledgeBase.OwnerUserId,
            knowledgeBase.VectorIndexId,
            knowledgeBase.EmbeddingModelId,
            knowledgeBase.ChunkingStrategy,
            knowledgeBase.ChunkSize,
            knowledgeBase.ChunkOverlap,
            knowledgeBase.EmbeddingDimension,
            knowledgeBase.IsActive,
            (knowledgeBase.Documents ?? new List<KnowledgeDocument>()).Count(d => !d.IsDeleted),
            knowledgeBase.StatisticsJson,
            knowledgeBase.CreatedAt,
            knowledgeBase.UpdatedAt);

    private static KnowledgeDocumentDto MapToDocumentDto(KnowledgeDocument document)
        => new(
            document.Id,
            document.KnowledgeBaseId,
            document.KnowledgeSourceId,
            document.Title,
            document.DocumentType,
            document.ContentHash,
            document.ExternalId,
            document.StoragePath,
            document.MimeType,
            document.PageCount,
            document.WordCount,
            document.Status,
            document.ProcessedAt,
            document.CreatedAt);
}
