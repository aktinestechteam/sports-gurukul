using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Conversation;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Commands.ConversationCommands;

public class CreateConversationCommandHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateConversationCommandHandler _handler;

    public CreateConversationCommandHandlerTests()
    {
        _handler = new CreateConversationCommandHandler(_service.Object, _unitOfWork.Object);
    }

    private static Conversation BuildConversation() => new()
    {
        Id = Guid.NewGuid(),
        Title = "Hello",
        AssistantId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        Status = ConversationStatus.Active,
        MessageCount = 1,
        CreatedAt = DateTime.UtcNow,
        Assistant = new AIAssistant { Id = Guid.NewGuid(), Name = "CoachAI" },
        Messages =
        [
            new ConversationMessage
            {
                Id = Guid.NewGuid(),
                ConversationId = Guid.NewGuid(),
                Role = MessageRole.User,
                Status = MessageStatus.Sent,
                Content = "Hi",
                CreatedAt = DateTime.UtcNow
            }
        ]
    };

    [Fact]
    public async Task Handle_ServiceSuccess_MapsToConversationDto()
    {
        var conversation = BuildConversation();
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreateConversationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var result = await _handler.Handle(
            new CreateConversationCommand("Hello", conversation.AssistantId, conversation.UserId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(conversation.Id);
        dto.Title.Should().Be("Hello");
        dto.AssistantName.Should().Be("CoachAI");
        dto.Status.Should().Be(ConversationStatus.Active);
        dto.MessageCount.Should().Be(1);
        dto.Messages.Should().HaveCount(1);
        dto.Messages[0].Content.Should().Be("Hi");
        dto.Messages[0].Role.Should().Be(MessageRole.User);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreateConversationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Failure("Conversation not created"));

        var result = await _handler.Handle(
            new CreateConversationCommand("Hello", null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not created");
    }

    [Fact]
    public async Task Handle_VerifiesRequestPassedToService()
    {
        var conversation = BuildConversation();
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreateConversationRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var assistantId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        await _handler.Handle(new CreateConversationCommand("Hello", assistantId, userId), CancellationToken.None);

        _service.Verify(s => s.CreateAsync(
            It.Is<CreateConversationRequest>(r =>
                r.Title == "Hello" && r.AssistantId == assistantId && r.UserId == userId),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class AddMessageCommandHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly AddMessageCommandHandler _handler;

    public AddMessageCommandHandlerTests()
    {
        _handler = new AddMessageCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_ServiceSuccess_MapsFromMessageConversation()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Title = "T",
            UserId = Guid.NewGuid(),
            Status = ConversationStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        var message = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Role = MessageRole.User,
            Status = MessageStatus.Sent,
            Content = "content",
            TokensUsed = 5,
            Metadata = "{}",
            CreatedAt = DateTime.UtcNow,
            Conversation = conversation
        };
        _service.Setup(s => s.AddMessageAsync(
                It.IsAny<AddMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ConversationMessage>.Success(message));

        var result = await _handler.Handle(
            new AddMessageCommand(conversation.Id, MessageRole.User, "content", "{}"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(conversation.Id);
        dto.Messages.Should().HaveCount(0);
    }

    [Fact]
    public async Task Handle_ServiceFailure_ReturnsFailure()
    {
        _service.Setup(s => s.AddMessageAsync(
                It.IsAny<AddMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ConversationMessage>.Failure("Conversation not found"));

        var result = await _handler.Handle(
            new AddMessageCommand(Guid.NewGuid(), MessageRole.User, "x", null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }

    [Fact]
    public async Task Handle_VerifiesRequestPassedToService()
    {
        var conversationId = Guid.NewGuid();
        var message = new ConversationMessage { Conversation = new Conversation { Id = conversationId } };
        _service.Setup(s => s.AddMessageAsync(
                It.IsAny<AddMessageRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ConversationMessage>.Success(message));

        await _handler.Handle(new AddMessageCommand(conversationId, MessageRole.Assistant, "body", null), CancellationToken.None);

        _service.Verify(s => s.AddMessageAsync(
            It.Is<AddMessageRequest>(r =>
                r.ConversationId == conversationId && r.Role == MessageRole.Assistant && r.Content == "body"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class ClearConversationMemoryCommandHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly ClearConversationMemoryCommandHandler _handler;

    public ClearConversationMemoryCommandHandlerTests()
    {
        _handler = new ClearConversationMemoryCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccessWithDefault()
    {
        var id = Guid.NewGuid();
        var conversation = new Conversation { Id = id, Status = ConversationStatus.Active, CreatedAt = DateTime.UtcNow };
        _service.Setup(s => s.ClearMemoryAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));
        _service.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var result = await _handler.Handle(new ClearConversationMemoryCommand(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.ClearMemoryAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Conversation not found"));

        var result = await _handler.Handle(new ClearConversationMemoryCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }
}

public class SummarizeConversationCommandHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly SummarizeConversationCommandHandler _handler;

    public SummarizeConversationCommandHandlerTests()
    {
        _handler = new SummarizeConversationCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccessWithDefault()
    {
        var id = Guid.NewGuid();
        var conversation = new Conversation { Id = id, Status = ConversationStatus.Active, CreatedAt = DateTime.UtcNow };
        _service.Setup(s => s.SummarizeAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Success("summary"));
        _service.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var result = await _handler.Handle(new SummarizeConversationCommand(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.SummarizeAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<string>.Failure("Conversation not found"));

        var result = await _handler.Handle(new SummarizeConversationCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }
}

public class RenameConversationCommandHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly RenameConversationCommandHandler _handler;

    public RenameConversationCommandHandlerTests()
    {
        _handler = new RenameConversationCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsConversation()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Title = "New Title",
            Status = ConversationStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.RenameAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var result = await _handler.Handle(new RenameConversationCommand(conversation.Id, "New Title"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("New Title");
        result.Value.Id.Should().Be(conversation.Id);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.RenameAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Failure("Conversation not found"));

        var result = await _handler.Handle(new RenameConversationCommand(Guid.NewGuid(), "x"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }
}

public class ArchiveConversationCommandHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly ArchiveConversationCommandHandler _handler;

    public ArchiveConversationCommandHandlerTests()
    {
        _handler = new ArchiveConversationCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsConversation()
    {
        var conversation = new Conversation
        {
            Id = Guid.NewGuid(),
            Status = ConversationStatus.Archived,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.ArchiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Success(conversation));

        var result = await _handler.Handle(new ArchiveConversationCommand(conversation.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(ConversationStatus.Archived);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.ArchiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<Conversation>.Failure("Conversation not found"));

        var result = await _handler.Handle(new ArchiveConversationCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class DeleteConversationCommandHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DeleteConversationCommandHandler _handler;

    public DeleteConversationCommandHandlerTests()
    {
        _handler = new DeleteConversationCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccess()
    {
        _service.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(new DeleteConversationCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Conversation not found"));

        var result = await _handler.Handle(new DeleteConversationCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Conversation not found");
    }
}

public class RegenerateResponseCommandHandlerTests
{
    private readonly Mock<IConversationService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly RegenerateResponseCommandHandler _handler;

    public RegenerateResponseCommandHandlerTests()
    {
        _handler = new RegenerateResponseCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.RegenerateResponseAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ConversationMessage>.Failure("Regenerate response not yet implemented"));

        var result = await _handler.Handle(
            new RegenerateResponseCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Regenerate response not yet implemented");
    }

    [Fact]
    public async Task Handle_Success_MapsMessage()
    {
        var conversation = new Conversation { Id = Guid.NewGuid(), CreatedAt = DateTime.UtcNow };
        var message = new ConversationMessage
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            Content = "gen",
            CreatedAt = DateTime.UtcNow,
            Conversation = conversation
        };
        _service.Setup(s => s.RegenerateResponseAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<ConversationMessage>.Success(message));

        var result = await _handler.Handle(
            new RegenerateResponseCommand(conversation.Id, message.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(conversation.Id);
    }
}
