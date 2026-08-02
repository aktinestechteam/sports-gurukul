using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Features.AIManagement.DomainEvents;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Services;

public class ConversationServiceTests
{
    private readonly Mock<IConversationRepository> _conversationRepo = new();
    private readonly Mock<IConversationMessageRepository> _messageRepo = new();
    private readonly Mock<IConversationMemoryRepository> _memoryRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly ConversationService _service;

    public ConversationServiceTests()
    {
        _service = new ConversationService(
            _conversationRepo.Object, _messageRepo.Object, _memoryRepo.Object,
            _publisher.Object, NullLogger<ConversationService>.Instance);
    }

    private static void SetupPublish<T>(Mock<IPublisher> publisher) where T : INotification
        => publisher.Setup(p => p.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

    [Fact]
    public async Task CreateAsync_CreatesActiveConversationAndPublishesEvent()
    {
        SetupPublish<ConversationCreatedEvent>(_publisher);
        var assistantId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        var result = await _service.CreateAsync(
            new CreateConversationRequest("Hello", assistantId, userId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var entity = result.Value!;
        entity.Title.Should().Be("Hello");
        entity.AssistantId.Should().Be(assistantId);
        entity.UserId.Should().Be(userId);
        entity.Status.Should().Be(ConversationStatus.Active);

        _conversationRepo.Verify(r => r.AddAsync(It.IsAny<Conversation>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisher.Verify(p => p.Publish(It.Is<ConversationCreatedEvent>(e =>
            e.ConversationId == entity.Id && e.AssistantId == assistantId), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RenameAsync_Success_UpdatesTitle()
    {
        var conversation = new Conversation { Id = Guid.NewGuid(), Title = "Old", CreatedAt = DateTime.UtcNow };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.RenameAsync(conversation.Id, "New", CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("New");
        result.Value.UpdatedAt.Should().NotBeNull();
        _conversationRepo.Verify(r => r.Update(conversation), Times.Once);
    }

    [Fact]
    public async Task RenameAsync_NotFound_ReturnsFailure()
    {
        _conversationRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var result = await _service.RenameAsync(Guid.NewGuid(), "x", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }

    [Fact]
    public async Task RenameAsync_Deleted_ReturnsFailure()
    {
        var conversation = new Conversation { Id = Guid.NewGuid(), IsDeleted = true };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.RenameAsync(conversation.Id, "x", CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task ArchiveAsync_Success_ArchivesAndPublishesEvent()
    {
        SetupPublish<ConversationArchivedEvent>(_publisher);
        var conversation = new Conversation { Id = Guid.NewGuid(), Status = ConversationStatus.Active };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.ArchiveAsync(conversation.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(ConversationStatus.Archived);
        _publisher.Verify(p => p.Publish(It.IsAny<ConversationArchivedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Success_MarksDeleted()
    {
        var conversation = new Conversation { Id = Guid.NewGuid() };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.DeleteAsync(conversation.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        conversation.IsDeleted.Should().BeTrue();
        _conversationRepo.Verify(r => r.Update(conversation), Times.Once);
    }

    [Fact]
    public async Task AddMessageAsync_Success_AddsMessageAndUpdatesConversation()
    {
        SetupPublish<MessageAddedEvent>(_publisher);
        var conversation = new Conversation { Id = Guid.NewGuid(), MessageCount = 0 };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.AddMessageAsync(
            new AddMessageRequest(conversation.Id, MessageRole.User, "content", "{}"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var message = result.Value!;
        message.ConversationId.Should().Be(conversation.Id);
        message.Role.Should().Be(MessageRole.User);
        message.Content.Should().Be("content");
        message.Status.Should().Be(MessageStatus.Sent);
        message.Metadata.Should().Be("{}");

        conversation.MessageCount.Should().Be(1);
        conversation.LastActivityAt.Should().NotBeNull();
        _messageRepo.Verify(r => r.AddAsync(It.IsAny<ConversationMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisher.Verify(p => p.Publish(It.IsAny<MessageAddedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddMessageAsync_ConversationNotFound_ReturnsFailure()
    {
        _conversationRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var result = await _service.AddMessageAsync(
            new AddMessageRequest(Guid.NewGuid(), MessageRole.User, "c", null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }

    [Fact]
    public async Task RegenerateResponseAsync_ConversationNotFound_ReturnsFailure()
    {
        _conversationRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var result = await _service.RegenerateResponseAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }

    [Fact]
    public async Task RegenerateResponseAsync_MessageNotFound_ReturnsFailure()
    {
        var conversation = new Conversation { Id = Guid.NewGuid() };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _messageRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ConversationMessage?)null);

        var result = await _service.RegenerateResponseAsync(conversation.Id, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Message not found");
    }

    [Fact]
    public async Task RegenerateResponseAsync_NotImplemented_ReturnsFailure()
    {
        var conversation = new Conversation { Id = Guid.NewGuid() };
        var message = new ConversationMessage { Id = Guid.NewGuid() };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _messageRepo.Setup(r => r.GetByIdAsync(message.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(message);

        var result = await _service.RegenerateResponseAsync(conversation.Id, message.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Regenerate response not yet implemented");
    }

    [Fact]
    public async Task ClearMemoryAsync_Success_RemovesAllMemories()
    {
        var conversation = new Conversation { Id = Guid.NewGuid() };
        var memories = new List<ConversationMemory>
        {
            new() { Id = Guid.NewGuid(), ConversationId = conversation.Id },
            new() { Id = Guid.NewGuid(), ConversationId = conversation.Id }
        };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);
        _memoryRepo.Setup(r => r.GetByConversationIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(memories);

        var result = await _service.ClearMemoryAsync(conversation.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _memoryRepo.Verify(r => r.Remove(It.IsAny<ConversationMemory>()), Times.Exactly(2));
    }

    [Fact]
    public async Task ClearMemoryAsync_NotFound_ReturnsFailure()
    {
        _conversationRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var result = await _service.ClearMemoryAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task SummarizeAsync_Success_SetsContextSummary()
    {
        var conversation = new Conversation { Id = Guid.NewGuid() };
        _conversationRepo.Setup(r => r.GetByIdAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.SummarizeAsync(conversation.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().StartWith("Conversation summary generated at");
        conversation.ContextSummary.Should().Be(result.Value);
    }

    [Fact]
    public async Task GetByIdAsync_Success_ReturnsConversation()
    {
        var conversation = new Conversation { Id = Guid.NewGuid() };
        _conversationRepo.Setup(r => r.GetByIdWithDetailsAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.GetByIdAsync(conversation.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(conversation.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsFailure()
    {
        _conversationRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task GetHistoryAsync_Success_ReturnsMessages()
    {
        var messages = new List<ConversationMessage> { new() { Id = Guid.NewGuid() } };
        _messageRepo.Setup(r => r.GetByConversationIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(messages);

        var result = await _service.GetHistoryAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }

    [Fact]
    public async Task SearchAsync_PassesFilterExpressionToRepository()
    {
        var expected = new List<Conversation> { new() { Id = Guid.NewGuid() } };
        _conversationRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Conversation, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _service.SearchAsync(
            new SearchConversationsRequest("term", Guid.NewGuid(), null, ConversationStatus.Active, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
        _conversationRepo.Verify(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Conversation, bool>>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
