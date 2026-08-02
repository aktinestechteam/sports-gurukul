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

public class ConversationService : IConversationService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IConversationMessageRepository _messageRepository;
    private readonly IConversationMemoryRepository _memoryRepository;
    private readonly IPublisher _publisher;
    private readonly ILogger<ConversationService> _logger;

    public ConversationService(
        IConversationRepository conversationRepository,
        IConversationMessageRepository messageRepository,
        IConversationMemoryRepository memoryRepository,
        IPublisher publisher,
        ILogger<ConversationService> logger)
    {
        _conversationRepository = conversationRepository;
        _messageRepository = messageRepository;
        _memoryRepository = memoryRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result<Conversation>> CreateAsync(CreateConversationRequest request, CancellationToken cancellationToken = default)
    {
        var entity = new Conversation
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            AssistantId = request.AssistantId,
            UserId = request.UserId,
            Status = ConversationStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        await _conversationRepository.AddAsync(entity, cancellationToken);

        await _publisher.Publish(new ConversationCreatedEvent(entity.Id, entity.AssistantId, entity.UserId, entity.CreatedAt), cancellationToken);

        _logger.LogInformation("Created conversation {ConversationId}", entity.Id);

        return Result<Conversation>.Success(entity);
    }

    public async Task<Result<Conversation>> RenameAsync(Guid id, string title, CancellationToken cancellationToken = default)
    {
        var entity = await _conversationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<Conversation>.Failure("Conversation not found");

        entity.Title = title;
        entity.UpdatedAt = DateTime.UtcNow;

        _conversationRepository.Update(entity);

        _logger.LogInformation("Renamed conversation {ConversationId} to {Title}", id, title);

        return Result<Conversation>.Success(entity);
    }

    public async Task<Result<Conversation>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _conversationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<Conversation>.Failure("Conversation not found");

        entity.Status = ConversationStatus.Archived;
        entity.UpdatedAt = DateTime.UtcNow;

        _conversationRepository.Update(entity);

        await _publisher.Publish(new ConversationArchivedEvent(entity.Id, DateTime.UtcNow), cancellationToken);

        _logger.LogInformation("Archived conversation {ConversationId}", id);

        return Result<Conversation>.Success(entity);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _conversationRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<bool>.Failure("Conversation not found");

        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;

        _conversationRepository.Update(entity);

        _logger.LogInformation("Deleted conversation {ConversationId}", id);

        return Result<bool>.Success(true);
    }

    public async Task<Result<ConversationMessage>> AddMessageAsync(AddMessageRequest request, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(request.ConversationId, cancellationToken);
        if (conversation is null || conversation.IsDeleted)
            return Result<ConversationMessage>.Failure("Conversation not found");

        var message = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = request.ConversationId,
            Role = request.Role,
            Content = request.Content,
            Status = MessageStatus.Sent,
            Metadata = request.Metadata,
            CreatedAt = DateTime.UtcNow
        };

        await _messageRepository.AddAsync(message, cancellationToken);

        conversation.MessageCount++;
        conversation.LastActivityAt = DateTime.UtcNow;
        conversation.UpdatedAt = DateTime.UtcNow;

        _conversationRepository.Update(conversation);

        await _publisher.Publish(new MessageAddedEvent(
            conversation.Id, message.Id, message.Role.ToString(), 0, message.CreatedAt), cancellationToken);

        _logger.LogInformation("Added message {MessageId} to conversation {ConversationId}", message.Id, request.ConversationId);

        return Result<ConversationMessage>.Success(message);
    }

    public async Task<Result<ConversationMessage>> RegenerateResponseAsync(Guid conversationId, Guid messageId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null || conversation.IsDeleted)
            return Result<ConversationMessage>.Failure("Conversation not found");

        var message = await _messageRepository.GetByIdAsync(messageId, cancellationToken);
        if (message is null)
            return Result<ConversationMessage>.Failure("Message not found");

        _logger.LogInformation("Regenerate response requested for message {MessageId} in conversation {ConversationId}", messageId, conversationId);

        return Result<ConversationMessage>.Failure("Regenerate response not yet implemented");
    }

    public async Task<Result<bool>> ClearMemoryAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (conversation is null || conversation.IsDeleted)
            return Result<bool>.Failure("Conversation not found");

        var memories = await _memoryRepository.GetByConversationIdAsync(conversationId, cancellationToken);

        foreach (var memory in memories)
        {
            _memoryRepository.Remove(memory);
        }

        _logger.LogInformation("Cleared {Count} memories for conversation {ConversationId}", memories.Count, conversationId);

        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> SummarizeAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var entity = await _conversationRepository.GetByIdAsync(conversationId, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<string>.Failure("Conversation not found");

        entity.ContextSummary = $"Conversation summary generated at {DateTime.UtcNow:O}";
        entity.UpdatedAt = DateTime.UtcNow;

        _conversationRepository.Update(entity);

        _logger.LogInformation("Summarized conversation {ConversationId}", conversationId);

        return Result<string>.Success(entity.ContextSummary);
    }

    public async Task<Result<Conversation>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _conversationRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<Conversation>.Failure("Conversation not found");

        return Result<Conversation>.Success(entity);
    }

    public async Task<Result<IReadOnlyList<ConversationMessage>>> GetHistoryAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var messages = await _messageRepository.GetByConversationIdAsync(conversationId, cancellationToken);

        return Result<IReadOnlyList<ConversationMessage>>.Success(messages);
    }

    public async Task<Result<IReadOnlyList<Conversation>>> SearchAsync(SearchConversationsRequest request, CancellationToken cancellationToken = default)
    {
        var query = await _conversationRepository.FindAsync(c =>
            !c.IsDeleted &&
            (string.IsNullOrEmpty(request.SearchTerm) || (c.Title != null && c.Title.Contains(request.SearchTerm))) &&
            (!request.AssistantId.HasValue || c.AssistantId == request.AssistantId) &&
            (!request.UserId.HasValue || c.UserId == request.UserId) &&
            (!request.Status.HasValue || c.Status == request.Status) &&
            (!request.FromDate.HasValue || c.CreatedAt >= request.FromDate.Value) &&
            (!request.ToDate.HasValue || c.CreatedAt <= request.ToDate.Value), cancellationToken);

        return Result<IReadOnlyList<Conversation>>.Success(query);
    }
}
