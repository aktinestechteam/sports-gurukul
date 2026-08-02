using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class AssistantService : IAssistantService
{
    private readonly IAIAssistantRepository _assistantRepository;
    private readonly IKnowledgeBaseRepository _knowledgeBaseRepository;
    private readonly IToolDefinitionRepository _toolRepository;
    private readonly IPublisher _publisher;
    private readonly ILogger<AssistantService> _logger;

    public AssistantService(
        IAIAssistantRepository assistantRepository,
        IKnowledgeBaseRepository knowledgeBaseRepository,
        IToolDefinitionRepository toolRepository,
        IPublisher publisher,
        ILogger<AssistantService> logger)
    {
        _assistantRepository = assistantRepository;
        _knowledgeBaseRepository = knowledgeBaseRepository;
        _toolRepository = toolRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result<AIAssistant>> CreateAsync(CreateAssistantRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new AIAssistant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            AssistantType = request.AssistantType,
            Personality = request.Personality,
            SystemPrompt = request.SystemPrompt,
            GreetingMessage = request.GreetingMessage,
            IsPublic = request.IsPublic,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _assistantRepository.AddAsync(entity, cancellationToken);

        _logger.LogInformation("Created assistant {AssistantId} with name {Name}", entity.Id, entity.Name);

        return Result<AIAssistant>.Success(entity);
    }

    public async Task<Result<AIAssistant>> UpdateAsync(UpdateAssistantRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _assistantRepository.GetByIdAsync(request.Id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<AIAssistant>.Failure("Assistant not found");

        if (request.Name is not null) entity.Name = request.Name;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.AssistantType.HasValue) entity.AssistantType = request.AssistantType.Value;
        if (request.Personality.HasValue) entity.Personality = request.Personality.Value;
        if (request.SystemPrompt is not null) entity.SystemPrompt = request.SystemPrompt;
        if (request.GreetingMessage is not null) entity.GreetingMessage = request.GreetingMessage;
        if (request.IsPublic.HasValue) entity.IsPublic = request.IsPublic.Value;
        entity.UpdatedAt = DateTime.UtcNow;

        _assistantRepository.Update(entity);

        _logger.LogInformation("Updated assistant {AssistantId}", request.Id);

        return Result<AIAssistant>.Success(entity);
    }

    public async Task<Result<AIAssistant>> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _assistantRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<AIAssistant>.Failure("Assistant not found");

        entity.IsActive = true;
        entity.UpdatedAt = DateTime.UtcNow;

        _assistantRepository.Update(entity);

        _logger.LogInformation("Published assistant {AssistantId}", id);

        return Result<AIAssistant>.Success(entity);
    }

    public async Task<Result<AIAssistant>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _assistantRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<AIAssistant>.Failure("Assistant not found");

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;

        _assistantRepository.Update(entity);

        _logger.LogInformation("Archived assistant {AssistantId}", id);

        return Result<AIAssistant>.Success(entity);
    }

    public async Task<Result<bool>> AssignKnowledgeBaseAsync(Guid assistantId, Guid knowledgeBaseId, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(assistantId, cancellationToken);
        if (assistant is null || assistant.IsDeleted)
            return Result<bool>.Failure("Assistant not found");

        var kb = await _knowledgeBaseRepository.GetByIdAsync(knowledgeBaseId, cancellationToken);
        if (kb is null || kb.IsDeleted)
            return Result<bool>.Failure("Knowledge base not found");

        assistant.UpdatedAt = DateTime.UtcNow;
        _assistantRepository.Update(assistant);

        _logger.LogInformation("Assigned knowledge base {KnowledgeBaseId} to assistant {AssistantId}", knowledgeBaseId, assistantId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> AssignToolsAsync(Guid assistantId, List<Guid> toolIds, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(assistantId, cancellationToken);
        if (assistant is null || assistant.IsDeleted)
            return Result<bool>.Failure("Assistant not found");

        assistant.UpdatedAt = DateTime.UtcNow;
        _assistantRepository.Update(assistant);

        _logger.LogInformation("Assigned {Count} tools to assistant {AssistantId}", toolIds.Count, assistantId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<AIAssistant>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _assistantRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<AIAssistant>.Failure("Assistant not found");

        return Result<AIAssistant>.Success(entity);
    }

    public async Task<Result<IReadOnlyList<AIAssistant>>> SearchAsync(SearchAssistantsRequest request, CancellationToken cancellationToken = default)
    {
        var query = await _assistantRepository.FindAsync(a =>
            !a.IsDeleted &&
            (string.IsNullOrEmpty(request.SearchTerm) || a.Name.Contains(request.SearchTerm) || (a.Description != null && a.Description.Contains(request.SearchTerm))) &&
            (!request.AssistantType.HasValue || a.AssistantType == request.AssistantType) &&
            (!request.IsActive.HasValue || a.IsActive == request.IsActive) &&
            (!request.IsPublic.HasValue || a.IsPublic == request.IsPublic), cancellationToken);

        return Result<IReadOnlyList<AIAssistant>>.Success(query);
    }
}
