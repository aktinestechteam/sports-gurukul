using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class AssistantService : IAssistantService
{
    private readonly IAssistantRepository _assistantRepository;
    private readonly IRepository<AIModel> _modelRepository;
    private readonly IKnowledgeBaseRepository _knowledgeBaseRepository;
    private readonly IRepository<ToolDefinition> _toolRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AssistantService> _logger;

    public AssistantService(
        IAssistantRepository assistantRepository,
        IRepository<AIModel> modelRepository,
        IKnowledgeBaseRepository knowledgeBaseRepository,
        IRepository<ToolDefinition> toolRepository,
        IUnitOfWork unitOfWork,
        ILogger<AssistantService> logger)
    {
        _assistantRepository = assistantRepository;
        _modelRepository = modelRepository;
        _knowledgeBaseRepository = knowledgeBaseRepository;
        _toolRepository = toolRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<AssistantDto>> CreateAsync(CreateAssistantRequest request, CancellationToken cancellationToken = default)
    {
        if (request.ModelId.HasValue)
        {
            var model = await _modelRepository.GetByIdAsync(request.ModelId.Value, cancellationToken);
            if (model is null || !model.IsActive)
                return Result<AssistantDto>.Failure("The referenced model does not exist or is not active");
        }

        var assistant = new AIAssistant
        {
            Name = request.Name,
            DisplayName = request.DisplayName,
            Description = request.Description,
            AssistantType = request.AssistantType,
            SystemPrompt = request.SystemPrompt,
            ModelId = request.ModelId,
            Temperature = request.Temperature,
            TopP = request.TopP,
            MaxTokens = request.MaxTokens,
            MemoryEnabled = request.MemoryEnabled,
            StreamingEnabled = request.StreamingEnabled,
            IsActive = true,
            OwnerType = request.OwnerType,
            OwnerUserId = request.OwnerUserId,
            AvatarUrl = request.AvatarUrl,
            GuardrailsJson = request.GuardrailsJson,
            MetadataJson = request.MetadataJson,
        };

        await _assistantRepository.AddAsync(assistant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("AI assistant created: {AssistantName} ({AssistantId})", assistant.Name, assistant.Id);
        return Result<AssistantDto>.Success(MapToDto(assistant));
    }

    public async Task<Result<AssistantDto>> UpdateAsync(UpdateAssistantRequest request, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId, cancellationToken);
        if (assistant is null)
            return Result<AssistantDto>.Failure("Assistant not found");

        if (request.ExpectedRowVersion is { Length: > 0 } && !assistant.RowVersion.SequenceEqual(request.ExpectedRowVersion))
            return Result<AssistantDto>.Failure("The assistant was modified by another user; please refresh and try again");

        if (!string.IsNullOrWhiteSpace(request.Name)) assistant.Name = request.Name;
        if (!string.IsNullOrWhiteSpace(request.DisplayName)) assistant.DisplayName = request.DisplayName;
        if (request.Description is not null) assistant.Description = request.Description;
        if (request.AssistantType.HasValue) assistant.AssistantType = request.AssistantType.Value;
        if (request.SystemPrompt is not null) assistant.SystemPrompt = request.SystemPrompt;
        if (request.ModelId.HasValue) assistant.ModelId = request.ModelId;
        if (request.Temperature.HasValue) assistant.Temperature = request.Temperature;
        if (request.TopP.HasValue) assistant.TopP = request.TopP;
        if (request.MaxTokens.HasValue) assistant.MaxTokens = request.MaxTokens;
        if (request.MemoryEnabled.HasValue) assistant.MemoryEnabled = request.MemoryEnabled.Value;
        if (request.StreamingEnabled.HasValue) assistant.StreamingEnabled = request.StreamingEnabled.Value;
        if (request.AvatarUrl is not null) assistant.AvatarUrl = request.AvatarUrl;
        if (request.GuardrailsJson is not null) assistant.GuardrailsJson = request.GuardrailsJson;

        assistant.UpdatedAt = DateTime.UtcNow;
        _assistantRepository.Update(assistant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AssistantDto>.Success(MapToDto(assistant));
    }

    public async Task<Result<AssistantDto>> PublishAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(assistantId, cancellationToken);
        if (assistant is null)
            return Result<AssistantDto>.Failure("Assistant not found");

        assistant.IsActive = true;
        SetMetadataFlag(assistant, "published", true);
        assistant.UpdatedAt = DateTime.UtcNow;
        _assistantRepository.Update(assistant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AssistantDto>.Success(MapToDto(assistant));
    }

    public async Task<Result<AssistantDto>> ArchiveAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(assistantId, cancellationToken);
        if (assistant is null)
            return Result<AssistantDto>.Failure("Assistant not found");

        assistant.IsActive = false;
        assistant.UpdatedAt = DateTime.UtcNow;
        _assistantRepository.Update(assistant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AssistantDto>.Success(MapToDto(assistant));
    }

    public async Task<Result<AssistantDto>> AssignKnowledgeBaseAsync(AssignKnowledgeBaseRequest request, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId, cancellationToken);
        if (assistant is null)
            return Result<AssistantDto>.Failure("Assistant not found");

        var distinctIds = request.KnowledgeBaseIds.Distinct().ToList();
        if (distinctIds.Count > 0)
        {
            var existing = await _knowledgeBaseRepository.FindAsync(kb => distinctIds.Contains(kb.Id) && !kb.IsDeleted, cancellationToken);
            if (existing.Count != distinctIds.Count)
                return Result<AssistantDto>.Failure("One or more knowledge bases could not be found");
        }

        var current = AssistantAssignmentStore.GetKnowledgeBaseIds(assistant);
        var next = request.ClearExisting
            ? distinctIds.ToHashSet()
            : current.Concat(distinctIds).ToHashSet();

        AssistantAssignmentStore.SetKnowledgeBaseIds(assistant, next);
        assistant.UpdatedAt = DateTime.UtcNow;
        _assistantRepository.Update(assistant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AssistantDto>.Success(MapToDto(assistant));
    }

    public async Task<Result<AssistantDto>> AssignToolsAsync(AssignToolsRequest request, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId, cancellationToken);
        if (assistant is null)
            return Result<AssistantDto>.Failure("Assistant not found");

        var distinctIds = request.ToolDefinitionIds.Distinct().ToList();
        if (distinctIds.Count > 0)
        {
            var existing = await _toolRepository.FindAsync(t => distinctIds.Contains(t.Id) && !t.IsDeleted, cancellationToken);
            if (existing.Count != distinctIds.Count)
                return Result<AssistantDto>.Failure("One or more tool definitions could not be found");
        }

        var current = AssistantAssignmentStore.GetToolIds(assistant);
        var next = request.ClearExisting
            ? distinctIds.ToHashSet()
            : current.Concat(distinctIds).ToHashSet();

        AssistantAssignmentStore.SetToolIds(assistant, next);
        assistant.UpdatedAt = DateTime.UtcNow;
        _assistantRepository.Update(assistant);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<AssistantDto>.Success(MapToDto(assistant));
    }

    public async Task<Result<AssistantDto>> GetByIdAsync(Guid assistantId, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdWithDetailsAsync(assistantId, cancellationToken);
        if (assistant is null)
            return Result<AssistantDto>.Failure("Assistant not found");

        return Result<AssistantDto>.Success(MapToDto(assistant));
    }

    public async Task<Result<IReadOnlyList<AssistantDto>>> SearchAsync(
        string? searchTerm,
        AIAssistantType? assistantType,
        Guid? ownerUserId,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<AIAssistant> assistants;
        if (assistantType.HasValue)
            assistants = await _assistantRepository.GetByTypeAsync(assistantType.Value, cancellationToken);
        else if (ownerUserId.HasValue)
            assistants = await _assistantRepository.GetByOwnerAsync(ownerUserId.Value, cancellationToken);
        else
            assistants = await _assistantRepository.GetAllAsync(cancellationToken);

        var query = assistants.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(a =>
                a.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                a.DisplayName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (isActive.HasValue)
            query = query.Where(a => a.IsActive == isActive.Value);

        var paged = query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<IReadOnlyList<AssistantDto>>.Success(paged.Select(MapToDto).ToList());
    }

    private static void SetMetadataFlag(AIAssistant assistant, string key, bool value)
    {
        var node = string.IsNullOrWhiteSpace(assistant.MetadataJson)
            ? new JsonObject()
            : JsonNode.Parse(assistant.MetadataJson) as JsonObject ?? new JsonObject();

        node[key] = value;
        assistant.MetadataJson = node.ToJsonString();
    }

    private static AssistantDto MapToDto(AIAssistant assistant)
        => new(
            assistant.Id,
            assistant.Name,
            assistant.DisplayName,
            assistant.Description,
            assistant.AssistantType,
            assistant.SystemPrompt,
            assistant.ModelId,
            assistant.Model?.Name,
            assistant.Temperature,
            assistant.TopP,
            assistant.MaxTokens,
            assistant.MemoryEnabled,
            assistant.StreamingEnabled,
            assistant.IsActive,
            assistant.OwnerType,
            assistant.OwnerUserId,
            assistant.AvatarUrl,
            assistant.GuardrailsJson,
            AssistantAssignmentStore.GetKnowledgeBaseIds(assistant),
            AssistantAssignmentStore.GetToolIds(assistant),
            assistant.CreatedAt,
            assistant.UpdatedAt);
}
