using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Queries;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Commands;

public class CreateConversationCommandHandlerTests
{
    private readonly Mock<IConversationService> _serviceMock;
    private readonly CreateConversationCommandHandler _handler;

    public CreateConversationCommandHandlerTests()
    {
        _serviceMock = new Mock<IConversationService>();
        _handler = new CreateConversationCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var assistantId = Guid.NewGuid();
        var participantId = Guid.NewGuid();
        var conversationId = Guid.NewGuid();
        var command = new CreateConversationCommand(assistantId, "First chat", AIResourceOwnerType.Athlete, participantId, null, null);
        var expected = Result<ConversationDto>.Success(new ConversationDto(
            conversationId, assistantId, "First chat", null, AIConversationStatus.Active,
            AIResourceOwnerType.Athlete, participantId, DateTime.UtcNow, null, 0, 0,
            new List<Guid>(), null, DateTime.UtcNow));

        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateConversationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected.Value);
        _serviceMock.Verify(s => s.CreateAsync(It.IsAny<CreateConversationRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        var command = new CreateConversationCommand(Guid.NewGuid(), "Title", AIResourceOwnerType.System, null, null, null);
        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateConversationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ConversationDto>.Failure("Assistant not found"));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }
}

public class AddMessageCommandHandlerTests
{
    private readonly Mock<IConversationService> _serviceMock;
    private readonly AddMessageCommandHandler _handler;

    public AddMessageCommandHandlerTests()
    {
        _serviceMock = new Mock<IConversationService>();
        _handler = new AddMessageCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var conversationId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var command = new AddMessageCommand(conversationId, AIMessageRole.User, AIMessageContentType.Text, "Hello", null, null, null, null, null, null, null);
        var expected = Result<MessageDto>.Success(new MessageDto(
            messageId, conversationId, 1, AIMessageRole.User, AIMessageContentType.Text,
            "Hello", null, null, null, null, null, null, null, DateTime.UtcNow));

        _serviceMock.Setup(s => s.AddMessageAsync(It.IsAny<AddMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _serviceMock.Verify(s => s.AddMessageAsync(It.IsAny<AddMessageRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class DeleteConversationCommandHandlerTests
{
    private readonly Mock<IConversationService> _serviceMock;
    private readonly DeleteConversationCommandHandler _handler;

    public DeleteConversationCommandHandlerTests()
    {
        _serviceMock = new Mock<IConversationService>();
        _handler = new DeleteConversationCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingConversation_ReturnsSuccess()
    {
        var conversationId = Guid.NewGuid();
        _serviceMock.Setup(s => s.DeleteAsync(conversationId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(new DeleteConversationCommand(conversationId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }
}

public class SearchConversationsQueryHandlerTests
{
    [Fact]
    public async Task Handle_ValidQuery_ReturnsSuccess()
    {
        var serviceMock = new Mock<IConversationService>();
        var handler = new SearchConversationsQueryHandler(serviceMock.Object);
        var conversationId = Guid.NewGuid();
        var assistantId = Guid.NewGuid();
        var summaries = new List<ConversationSummaryDto>
        {
            new(conversationId, assistantId, "Chat", AIConversationStatus.Active, null, 3, 100, DateTime.UtcNow, DateTime.UtcNow)
        };
        serviceMock.Setup(s => s.SearchAsync(It.IsAny<string?>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<AIConversationStatus?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<IReadOnlyList<ConversationSummaryDto>>.Success(summaries));

        var result = await handler.Handle(new SearchConversationsQuery("chat", null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }
}
