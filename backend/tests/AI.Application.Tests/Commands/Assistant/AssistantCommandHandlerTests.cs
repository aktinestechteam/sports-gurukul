using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Assistant;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Commands.Assistant;

public class CreateAssistantCommandHandlerTests
{
    private readonly Mock<IAssistantService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateAssistantCommandHandler _handler;

    public CreateAssistantCommandHandlerTests()
    {
        _handler = new CreateAssistantCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAssistantDto()
    {
        var assistant = new AIAssistant
        {
            Id = Guid.NewGuid(),
            Name = "Coach",
            AssistantType = AIAssistantType.Coach,
            Personality = AIAssistantPersonality.Enthusiastic,
            IsPublic = true,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreateAssistantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Success(assistant));

        var result = await _handler.Handle(
            new CreateAssistantCommand("Coach", null, AIAssistantType.Coach,
                AIAssistantPersonality.Enthusiastic, null, null, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(assistant.Id);
        dto.Name.Should().Be("Coach");
        dto.AssistantType.Should().Be(AIAssistantType.Coach);
        dto.Personality.Should().Be(AIAssistantPersonality.Enthusiastic);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreateAssistantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Failure("Assistant creation failed"));

        var result = await _handler.Handle(
            new CreateAssistantCommand("x", null, AIAssistantType.Coach,
                AIAssistantPersonality.Enthusiastic, null, null, false),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant creation failed");
    }
}

public class UpdateAssistantCommandHandlerTests
{
    private readonly Mock<IAssistantService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateAssistantCommandHandler _handler;

    public UpdateAssistantCommandHandlerTests()
    {
        _handler = new UpdateAssistantCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAssistantDto()
    {
        var assistant = new AIAssistant
        {
            Id = Guid.NewGuid(),
            Name = "Updated",
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateAssistantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Success(assistant));

        var result = await _handler.Handle(
            new UpdateAssistantCommand(assistant.Id, "Updated", null, null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(assistant.Id);
        result.Value.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateAssistantRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Failure("Assistant not found"));

        var result = await _handler.Handle(
            new UpdateAssistantCommand(Guid.NewGuid(), "x", null, null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class PublishAssistantCommandHandlerTests
{
    private readonly Mock<IAssistantService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly PublishAssistantCommandHandler _handler;

    public PublishAssistantCommandHandlerTests()
    {
        _handler = new PublishAssistantCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAssistantDto()
    {
        var assistant = new AIAssistant
        {
            Id = Guid.NewGuid(),
            Name = "A",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Success(assistant));

        var result = await _handler.Handle(new PublishAssistantCommand(assistant.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Failure("Assistant not found"));

        var result = await _handler.Handle(new PublishAssistantCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class ArchiveAssistantCommandHandlerTests
{
    private readonly Mock<IAssistantService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly ArchiveAssistantCommandHandler _handler;

    public ArchiveAssistantCommandHandlerTests()
    {
        _handler = new ArchiveAssistantCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAssistantDto()
    {
        var assistant = new AIAssistant
        {
            Id = Guid.NewGuid(),
            Name = "A",
            IsActive = false,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.ArchiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Success(assistant));

        var result = await _handler.Handle(new ArchiveAssistantCommand(assistant.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.ArchiveAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AIAssistant>.Failure("Assistant not found"));

        var result = await _handler.Handle(new ArchiveAssistantCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class AssignKnowledgeBaseCommandHandlerTests
{
    private readonly Mock<IAssistantService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly AssignKnowledgeBaseCommandHandler _handler;

    public AssignKnowledgeBaseCommandHandlerTests()
    {
        _handler = new AssignKnowledgeBaseCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccessDefault()
    {
        _service.Setup(s => s.AssignKnowledgeBaseAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(
            new AssignKnowledgeBaseCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.AssignKnowledgeBaseAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Assistant not found"));

        var result = await _handler.Handle(
            new AssignKnowledgeBaseCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }
}

public class AssignToolsCommandHandlerTests
{
    private readonly Mock<IAssistantService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly AssignToolsCommandHandler _handler;

    public AssignToolsCommandHandlerTests()
    {
        _handler = new AssignToolsCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccessDefault()
    {
        _service.Setup(s => s.AssignToolsAsync(
                It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(
            new AssignToolsCommand(Guid.NewGuid(), [Guid.NewGuid()]), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.AssignToolsAsync(
                It.IsAny<Guid>(), It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Assistant not found"));

        var result = await _handler.Handle(
            new AssignToolsCommand(Guid.NewGuid(), []), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assistant not found");
    }
}
