using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Events;
using SportsGurukul.Application.Features.AIManagement.ModelRouting;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Services;

public class ConversationServiceTests
{
    private readonly Mock<IConversationRepository> _conversationRepoMock = new();
    private readonly Mock<IRepository<ConversationMessage>> _messageRepoMock = new();
    private readonly Mock<IRepository<ConversationMemory>> _memoryRepoMock = new();
    private readonly Mock<IAssistantRepository> _assistantRepoMock = new();
    private readonly Mock<IModelRoutingService> _routingMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly Mock<ILogger<ConversationService>> _loggerMock = new();
    private readonly ConversationService _service;

    public ConversationServiceTests()
    {
        _service = new ConversationService(
            _conversationRepoMock.Object,
            _messageRepoMock.Object,
            _memoryRepoMock.Object,
            _assistantRepoMock.Object,
            _routingMock.Object,
            _unitOfWorkMock.Object,
            _mediatorMock.Object,
            _loggerMock.Object);
    }

    private static Conversation BuildConversation(
        AIConversationStatus status = AIConversationStatus.Active,
        int messageCount = 0,
        int tokenCount = 0,
        AIModel? model = null) => new()
    {
        Id = Guid.NewGuid(),
        AssistantId = Guid.NewGuid(),
        Title = "Chat",
        Status = status,
        MessageCount = messageCount,
        TokenCount = tokenCount,
        ParticipantType = AIResourceOwnerType.Athlete,
        ParticipantUserId = Guid.NewGuid(),
        Assistant = model is null ? null : new AIAssistant { Id = Guid.NewGuid(), ModelId = model.Id, IsActive = true },
    };

    [Fact]
    public async Task CreateAsync_MissingAssistant_ReturnsFailure()
    {
        var assistantId = Guid.NewGuid();
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistantId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AIAssistant?)null);

        var result = await _service.CreateAsync(new CreateConversationRequest(
            assistantId, "Chat", AIResourceOwnerType.Athlete, Guid.NewGuid(), null, null));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }

    [Fact]
    public async Task CreateAsync_InactiveAssistant_ReturnsFailure()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), IsActive = false };
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);

        var result = await _service.CreateAsync(new CreateConversationRequest(
            assistant.Id, "Chat", AIResourceOwnerType.Athlete, null, null, null));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("inactive");
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_AddsConversationAndPublishesEvent()
    {
        var assistant = new AIAssistant { Id = Guid.NewGuid(), IsActive = true };
        _assistantRepoMock.Setup(r => r.GetByIdAsync(assistant.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assistant);

        var result = await _service.CreateAsync(new CreateConversationRequest(
            assistant.Id, "Chat", AIResourceOwnerType.Athlete, null, null, null));

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(AIConversationStatus.Active);
        _conversationRepoMock.Verify(r => r.AddAsync(It.Is<Conversation>(c => c.AssistantId == assistant.Id), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(
            It.IsAny<ConversationCreatedEvent>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddMessageAsync_ConversationNotFound_ReturnsFailure()
    {
        var conversationId = Guid.NewGuid();
        _conversationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Conversation?)null);

        var result = await _service.AddMessageAsync(new AddMessageRequest(
            conversationId, AIMessageRole.User, AIMessageContentType.Text, "Hello", null, null, null, null, null, null, null));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }

    [Fact]
    public async Task AddMessageAsync_DeletedConversation_ReturnsFailure()
    {
        var conversation = BuildConversation();
        conversation.IsDeleted = true;
        _conversationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.AddMessageAsync(new AddMessageRequest(
            conversation.Id, AIMessageRole.User, AIMessageContentType.Text, "Hello", null, null, null, null, null, null, null));

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("deleted");
    }

    [Fact]
    public async Task AddMessageAsync_ValidRequest_SequencesAndPublishesEvent()
    {
        var conversation = BuildConversation(messageCount: 1, tokenCount: 10);
        var existing = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SequenceNumber = 1,
            Role = AIMessageRole.User,
            ContentType = AIMessageContentType.Text,
            Content = "First",
        };
        conversation.Messages.Add(existing);
        _conversationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.AddMessageAsync(new AddMessageRequest(
            conversation.Id, AIMessageRole.Assistant, AIMessageContentType.Text, "Second", null, null, null, null, null, null, null));

        result.IsSuccess.Should().BeTrue();
        result.Value.SequenceNumber.Should().Be(2);
        conversation.MessageCount.Should().Be(2);
        conversation.LastMessageAt.Should().NotBeNull();
        _messageRepoMock.Verify(r => r.AddAsync(It.Is<ConversationMessage>(m => m.SequenceNumber == 2), It.IsAny<CancellationToken>()), Times.Once);
        _mediatorMock.Verify(m => m.Publish(
            It.Is<MessageAddedEvent>(e => e.SequenceNumber == 2),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ValidConversation_SoftDeletes()
    {
        var conversation = BuildConversation();
        _conversationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.DeleteAsync(conversation.Id);

        result.IsSuccess.Should().BeTrue();
        conversation.Status.Should().Be(AIConversationStatus.Deleted);
        conversation.IsDeleted.Should().BeTrue();
        _conversationRepoMock.Verify(r => r.Update(conversation), Times.Once);
    }

    [Fact]
    public async Task SummarizeAsync_ExistingMemory_UpdatesIt()
    {
        var conversation = BuildConversation();
        var memory = new ConversationMemory
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Key = "conversation_summary",
            Content = "old",
            MemoryType = AIMemoryType.Summary,
        };
        conversation.Memories.Add(memory);
        _conversationRepoMock.Setup(r => r.GetByIdWithDetailsAsync(conversation.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(conversation);

        var result = await _service.SummarizeAsync(new SummarizeConversationRequest(conversation.Id, "new summary"));

        result.IsSuccess.Should().BeTrue();
        memory.Content.Should().Be("new summary");
        memory.Importance.Should().Be(10);
        _memoryRepoMock.Verify(r => r.Update(memory), Times.Once);
    }
}
