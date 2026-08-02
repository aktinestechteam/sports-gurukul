using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Prompt;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Commands.Prompt;

public class CreatePromptTemplateCommandHandlerTests
{
    private readonly Mock<IPromptService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreatePromptTemplateCommandHandler _handler;

    public CreatePromptTemplateCommandHandlerTests()
    {
        _handler = new CreatePromptTemplateCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsPromptTemplateDto()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Intro",
            Type = PromptType.System,
            Status = PromptStatus.Draft,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreatePromptTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await _handler.Handle(
            new CreatePromptTemplateCommand("Intro", null, PromptType.System, "content", null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(template.Id);
        dto.Name.Should().Be("Intro");
        dto.Status.Should().Be(PromptStatus.Draft);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreatePromptTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Failure("Prompt template creation failed"));

        var result = await _handler.Handle(
            new CreatePromptTemplateCommand("x", null, PromptType.System, "c", null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Prompt template creation failed");
    }
}

public class UpdatePromptTemplateCommandHandlerTests
{
    private readonly Mock<IPromptService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdatePromptTemplateCommandHandler _handler;

    public UpdatePromptTemplateCommandHandlerTests()
    {
        _handler = new UpdatePromptTemplateCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsPromptTemplateDto()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Updated",
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.UpdateAsync(
                It.IsAny<UpdatePromptTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await _handler.Handle(
            new UpdatePromptTemplateCommand(template.Id, "Updated", null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(template.Id);
        result.Value.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.UpdateAsync(
                It.IsAny<UpdatePromptTemplateRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Failure("Prompt template not found"));

        var result = await _handler.Handle(
            new UpdatePromptTemplateCommand(Guid.NewGuid(), "x", null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class PublishPromptTemplateCommandHandlerTests
{
    private readonly Mock<IPromptService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly PublishPromptTemplateCommandHandler _handler;

    public PublishPromptTemplateCommandHandlerTests()
    {
        _handler = new PublishPromptTemplateCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsPromptTemplateDto()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            Name = "A",
            Status = PromptStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await _handler.Handle(new PublishPromptTemplateCommand(template.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(PromptStatus.Active);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.PublishAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Failure("Prompt template not found"));

        var result = await _handler.Handle(new PublishPromptTemplateCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class ClonePromptCommandHandlerTests
{
    private readonly Mock<IPromptService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly ClonePromptCommandHandler _handler;

    public ClonePromptCommandHandlerTests()
    {
        _handler = new ClonePromptCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsPromptTemplateDto()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            Name = "Clone",
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.CloneAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await _handler.Handle(
            new ClonePromptCommand(Guid.NewGuid(), "Clone"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Clone");
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.CloneAsync(
                It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Failure("Prompt template not found"));

        var result = await _handler.Handle(
            new ClonePromptCommand(Guid.NewGuid(), "x"), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Prompt template not found");
    }
}

public class RollbackPromptVersionCommandHandlerTests
{
    private readonly Mock<IPromptService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly RollbackPromptVersionCommandHandler _handler;

    public RollbackPromptVersionCommandHandlerTests()
    {
        _handler = new RollbackPromptVersionCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsPromptTemplateDto()
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            Name = "A",
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.RollbackAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Success(template));

        var result = await _handler.Handle(
            new RollbackPromptVersionCommand(Guid.NewGuid(), 2), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(template.Id);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.RollbackAsync(
                It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<PromptTemplate>.Failure("Prompt template not found"));

        var result = await _handler.Handle(
            new RollbackPromptVersionCommand(Guid.NewGuid(), 2), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}
