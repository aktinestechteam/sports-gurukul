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

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IRepository<ConversationMessage> _messageRepository;
    private readonly IRepository<ConversationMemory> _memoryRepository;
    private readonly IAssistantRepository _assistantRepository;
    private readonly IModelRoutingService _modelRoutingService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(
        IConversationRepository conversationRepository,
        IRepository<ConversationMessage> messageRepository,
        IRepository<ConversationMemory> memoryRepository,
        IAssistantRepository assistantRepository,
        IModelRoutingService modelRoutingService,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<ConversationService> logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _memoryRepository = memoryRepository;
        _assistantRepository = assistantRepository;
        _modelRoutingService = modelRoutingService;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<ConversationDto>> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId, cancellationToken);
        if (assistant is null)
            return Result<ConversationDto>.Failure("Assistant not found");

        if (!assistant.IsActive)
            return Result<ConversationDto>.Failure("Cannot create a conversation with an inactive assistant");

        var conversation = new Conversation
        {
            AssistantId = request.AssistantId,
            Title = request.Title,
            Status = AIConversationStatus.Active,
            ParticipantType = request.ParticipantType,
            ParticipantUserId = request.ParticipantUserId,
            StartedAt = DateTime.UtcNow,
            LastMessageAt = null,
            MessageCount = 0,
            TokenCount = 0,
            KnowledgeBaseIdsJson = request.KnowledgeBaseIds is { Count: > 0 } ? AiJson.Serialize(request.KnowledgeBaseIds) : null,
            ContextMetadataJson = request.ContextMetadataJson,
        };

        await _conversationRepository.AddAsync(conversation, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new ConversationCreatedEvent(conversation.Id, conversation.AssistantId, conversation.ParticipantUserId, DateTime.UtcNow),
            cancellationToken);

        return Result<ConversationDto>.Success(MapToDto(conversation));
    }

    public async Task<Result<ConversationDto>> RenameAsync(Guid conversationId, string title, CancellationToken cancellationToken = default)
    {
        var conversation = await GetActiveConversationAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<ConversationDto>.Failure("Conversation not found");

        if (conversation.Status is AIConversationStatus.Archived or AIConversationStatus.Deleted)
            return Result<ConversationDto>.Failure("Cannot rename an archived or deleted conversation");

        conversation.Title = title;
        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ConversationDto>.Success(MapToDto(conversation));
    }

    public async Task<Result<ConversationDto>> ArchiveAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await GetActiveConversationAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<ConversationDto>.Failure("Conversation not found");

        if (conversation.Status == AIConversationStatus.Archived)
            return Result<ConversationDto>.Failure("Conversation is already archived");

        conversation.Status = AIConversationStatus.Archived;
        conversation.ArchivedAt = DateTime.UtcNow;
        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new ConversationArchivedEvent(conversation.Id, conversation.AssistantId, DateTime.UtcNow),
            cancellationToken);

        return Result<ConversationDto>.Success(MapToDto(conversation));
    }

    public async Task<Result<bool>> DeleteAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await GetActiveConversationAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<bool>.Failure("Conversation not found");

        conversation.Status = AIConversationStatus.Deleted;
        conversation.IsDeleted = true;
        conversation.ArchivedAt = DateTime.UtcNow;
        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<MessageDto>> AddMessageAsync(AddMessageRequest request, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdWithDetailsAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
            return Result<MessageDto>.Failure("Conversation not found");

        if (conversation.IsDeleted)
            return Result<MessageDto>.Failure("Conversation has been deleted");

        if (conversation.Status != AIConversationStatus.Active)
            return Result<MessageDto>.Failure("Messages can only be added to active conversations");

        var sequenceNumber = conversation.Messages.Count == 0
            ? 1
            : conversation.Messages.Max(m => m.SequenceNumber) + 1;

        var message = new ConversationMessage
        {
            ConversationId = conversation.Id,
            SequenceNumber = sequenceNumber,
            Role = request.Role,
            ContentType = request.ContentType,
            Content = request.Content,
            ModelName = request.ModelName,
            PromptVersionUsed = request.PromptVersionUsed,
            InputTokenCount = request.InputTokenCount,
            OutputTokenCount = request.OutputTokenCount,
            LatencyMs = request.LatencyMs,
            ToolCallsJson = request.ToolCallsJson,
            ToolResultsJson = request.ToolResultsJson,
        };

        await _messageRepository.AddAsync(message, cancellationToken);

        var estimatedTokens = EstimateTokens(message);
        conversation.MessageCount += 1;
        conversation.LastMessageAt = DateTime.UtcNow;
        conversation.TokenCount += estimatedTokens;

        await ApplyContextWindowAsync(conversation, cancellationToken);

        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new MessageAddedEvent(conversation.Id, message.Id, conversation.AssistantId, message.Role, message.SequenceNumber, DateTime.UtcNow),
            cancellationToken);

        return Result<MessageDto>.Success(MapToMessageDto(message));
    }

    public async Task<Result<MessageDto>> RegenerateResponseAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdWithMessagesAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<MessageDto>.Failure("Conversation not found");

        var lastAssistantMessage = conversation.Messages
            .Where(m => m.Role == AIMessageRole.Assistant)
            .OrderByDescending(m => m.SequenceNumber)
            .FirstOrDefault();

        if (lastAssistantMessage is null)
            return Result<MessageDto>.Failure("No assistant response is available to regenerate");

        return Result<MessageDto>.Success(MapToMessageDto(lastAssistantMessage));
    }

    public async Task<Result<bool>> ClearMemoryAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<bool>.Failure("Conversation not found");

        var memories = conversation.Memories.ToList();
        foreach (var memory in memories)
        {
            _memoryRepository.Remove(memory);
            conversation.Memories.Remove(memory);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<ConversationDto>> SummarizeAsync(SummarizeConversationRequest request, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdWithDetailsAsync(request.ConversationId, cancellationToken);
        if (conversation is null)
            return Result<ConversationDto>.Failure("Conversation not found");

        conversation.Summary = request.Summary;

        var existing = conversation.Memories.FirstOrDefault(m => m.Key == "conversation_summary");
        if (existing is not null)
        {
            existing.Content = request.Summary;
            existing.Importance = 10;
            _memoryRepository.Update(existing);
        }
        else
        {
            var memory = new ConversationMemory
            {
                ConversationId = conversation.Id,
                MemoryType = AIMemoryType.Summary,
                Key = "conversation_summary",
                Content = request.Summary,
                Importance = 10,
                ExpiresAt = null,
            };
            await _memoryRepository.AddAsync(memory, cancellationToken);
        }

        _conversationRepository.Update(conversation);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<ConversationDto>.Success(MapToDto(conversation));
    }

    public async Task<Result<ConversationDto>> GetByIdAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await GetActiveConversationAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<ConversationDto>.Failure("Conversation not found");

        return Result<ConversationDto>.Success(MapToDto(conversation));
    }

    public async Task<Result<IReadOnlyList<MessageDto>>> GetHistoryAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdWithMessagesAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<IReadOnlyList<MessageDto>>.Failure("Conversation not found");

        var messages = conversation.Messages
            .OrderBy(m => m.SequenceNumber)
            .Select(MapToMessageDto)
            .ToList();

        return Result<IReadOnlyList<MessageDto>>.Success(messages);
    }

    public async Task<Result<IReadOnlyList<ConversationSummaryDto>>> SearchAsync(
        string? searchTerm,
        Guid? assistantId,
        Guid? participantUserId,
        AIConversationStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<Conversation> conversations;
        if (assistantId.HasValue)
            conversations = await _conversationRepository.GetByAssistantIdAsync(assistantId.Value, cancellationToken);
        else if (participantUserId.HasValue)
            conversations = await _conversationRepository.GetByParticipantAsync(participantUserId.Value, cancellationToken);
        else if (status.HasValue)
            conversations = await _conversationRepository.GetByStatusAsync(status.Value, cancellationToken);
        else
            conversations = await _conversationRepository.GetAllAsync(cancellationToken);

        var filtered = conversations.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            filtered = filtered.Where(c =>
                c.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (c.Summary?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        var paged = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<IReadOnlyList<ConversationSummaryDto>>.Success(paged.Select(MapToSummaryDto).ToList());
    }

    private async Task<Conversation?> GetActiveConversationAsync(Guid conversationId, CancellationToken cancellationToken)
        => await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);

    private async Task ApplyContextWindowAsync(Conversation conversation, CancellationToken cancellationToken)
    {
        var modelId = conversation.Assistant?.ModelId;
        if (!modelId.HasValue)
            return;

        var modelResult = await _modelRoutingService.GetModelCandidateAsync(modelId.Value, cancellationToken);
        if (!modelResult.IsSuccess || modelResult.Value is null || !modelResult.Value.ContextWindow.HasValue)
            return;

        var contextWindow = modelResult.Value.ContextWindow.Value;
        var estimated = conversation.Messages.Sum(EstimateTokens);
        if (estimated <= contextWindow)
            return;

        _logger.LogInformation(
            "Conversation {ConversationId} estimated at {Estimated} tokens exceeds context window {ContextWindow}; trimming older messages",
            conversation.Id,
            estimated,
            contextWindow);

        foreach (var message in conversation.Messages.OrderBy(m => m.SequenceNumber).ToList())
        {
            if (estimated <= contextWindow)
                break;

            if (message.Role == AIMessageRole.System)
                continue;

            estimated -= EstimateTokens(message);
            _messageRepository.Remove(message);
            conversation.Messages.Remove(message);
        }
    }

    private static int EstimateTokens(ConversationMessage message)
        => (int)Math.Ceiling((message.Content?.Length ?? 0) / 4d);

    private static ConversationDto MapToDto(Conversation conversation)
    {
        var knowledgeBaseIds = conversation.KnowledgeBaseIdsJson is null
            ? new List<Guid>()
            : AiJson.Deserialize<List<Guid>>(conversation.KnowledgeBaseIdsJson) ?? new List<Guid>();

        return new ConversationDto(
            conversation.Id,
            conversation.AssistantId,
            conversation.Title,
            conversation.Summary,
            conversation.Status,
            conversation.ParticipantType,
            conversation.ParticipantUserId,
            conversation.StartedAt,
            conversation.LastMessageAt,
            conversation.MessageCount,
            conversation.TokenCount,
            knowledgeBaseIds,
            conversation.ArchivedAt,
            conversation.CreatedAt);
    }

    private static ConversationSummaryDto MapToSummaryDto(Conversation conversation)
        => new(
            conversation.Id,
            conversation.AssistantId,
            conversation.Title,
            conversation.Status,
            conversation.ParticipantUserId,
            conversation.MessageCount,
            conversation.TokenCount,
            conversation.LastMessageAt,
            conversation.UpdatedAt);

    private static MessageDto MapToMessageDto(ConversationMessage message)
        => new(
            message.Id,
            message.ConversationId,
            message.SequenceNumber,
            message.Role,
            message.ContentType,
            message.Content,
            message.ModelName,
            message.PromptVersionUsed,
            message.InputTokenCount,
            message.OutputTokenCount,
            message.LatencyMs,
            message.ToolCallsJson,
            message.ToolResultsJson,
            message.CreatedAt);
}
