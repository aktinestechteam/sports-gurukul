using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class ConversationMemoryService : IConversationMemoryService
{
    private readonly IConversationRepository _conversationRepository;
    private readonly IRepository<ConversationMemory> _memoryRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ConversationMemoryService> _logger;

    public ConversationMemoryService(
        IConversationRepository conversationRepository,
        IRepository<ConversationMemory> memoryRepository,
        IUnitOfWork unitOfWork,
        ILogger<ConversationMemoryService> logger)
    {
        _conversationRepository = conversationRepository;
        _memoryRepository = memoryRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> ClearAsync(Guid conversationId, CancellationToken cancellationToken = default)
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
        _logger.LogInformation("Cleared {Count} memory entries for conversation {ConversationId}", memories.Count, conversationId);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> StoreAsync(
        Guid conversationId,
        AIMemoryType memoryType,
        string key,
        string content,
        int importance,
        DateTime? expiresAt,
        CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<bool>.Failure("Conversation not found");

        var existing = conversation.Memories.FirstOrDefault(m => m.Key == key);
        if (existing is not null)
        {
            existing.MemoryType = memoryType;
            existing.Content = content;
            existing.Importance = importance;
            existing.ExpiresAt = expiresAt;
            _memoryRepository.Update(existing);
        }
        else
        {
            var memory = new ConversationMemory
            {
                ConversationId = conversationId,
                MemoryType = memoryType,
                Key = key,
                Content = content,
                Importance = importance,
                ExpiresAt = expiresAt,
            };
            await _memoryRepository.AddAsync(memory, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IReadOnlyList<ConversationMemoryDto>>> GetAsync(Guid conversationId, CancellationToken cancellationToken = default)
    {
        var conversation = await _conversationRepository.GetByIdWithDetailsAsync(conversationId, cancellationToken);
        if (conversation is null)
            return Result<IReadOnlyList<ConversationMemoryDto>>.Failure("Conversation not found");

        var now = DateTime.UtcNow;
        var memories = conversation.Memories
            .Where(m => !m.IsDeleted && (m.ExpiresAt is null || m.ExpiresAt > now))
            .OrderByDescending(m => m.Importance)
            .Select(m => new ConversationMemoryDto(
                m.Id,
                m.ConversationId,
                m.MemoryType,
                m.Key,
                m.Content,
                m.Importance,
                m.ExpiresAt,
                m.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<ConversationMemoryDto>>.Success(memories);
    }
}
