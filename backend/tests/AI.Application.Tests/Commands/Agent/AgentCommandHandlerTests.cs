using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Agent;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Commands.Agent;

public class CreateAgentCommandHandlerTests
{
    private readonly Mock<IAgentService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateAgentCommandHandler _handler;

    public CreateAgentCommandHandlerTests()
    {
        _handler = new CreateAgentCommandHandler(_service.Object, _unitOfWork.Object);
    }

    private static AgentDefinition BuildAgent() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Analyst",
        Status = AgentStatus.Draft,
        MaxIterations = 5,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task Handle_Success_MapsAgentDto()
    {
        var agent = BuildAgent();
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreateAgentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Success(agent));

        var result = await _handler.Handle(
            new CreateAgentCommand("Analyst", "desc", null, null, null, null, null, 5, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(agent.Id);
        dto.Name.Should().Be("Analyst");
        dto.Status.Should().Be(AgentStatus.Draft);
        dto.MaxIterations.Should().Be(5);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.CreateAsync(
                It.IsAny<CreateAgentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Failure("Agent creation failed"));

        var result = await _handler.Handle(new CreateAgentCommand("x", null, null, null, null, null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent creation failed");
    }
}

public class UpdateAgentCommandHandlerTests
{
    private readonly Mock<IAgentService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateAgentCommandHandler _handler;

    public UpdateAgentCommandHandlerTests()
    {
        _handler = new UpdateAgentCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAgentDto()
    {
        var agent = new AgentDefinition
        {
            Id = Guid.NewGuid(),
            Name = "Updated",
            Status = AgentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateAgentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Success(agent));

        var result = await _handler.Handle(
            new UpdateAgentCommand(agent.Id, "Updated", "d", null, null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Updated");
        result.Value.Id.Should().Be(agent.Id);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.UpdateAsync(
                It.IsAny<UpdateAgentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Failure("Agent not found"));

        var result = await _handler.Handle(new UpdateAgentCommand(Guid.NewGuid(), "x", null, null, null, null, null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent not found");
    }
}

public class EnableAgentCommandHandlerTests
{
    private readonly Mock<IAgentService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly EnableAgentCommandHandler _handler;

    public EnableAgentCommandHandlerTests()
    {
        _handler = new EnableAgentCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAgentDto()
    {
        var agent = new AgentDefinition
        {
            Id = Guid.NewGuid(),
            Name = "A",
            Status = AgentStatus.Active,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.EnableAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Success(agent));

        var result = await _handler.Handle(new EnableAgentCommand(agent.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(AgentStatus.Active);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.EnableAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Failure("Agent not found"));

        var result = await _handler.Handle(new EnableAgentCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class DisableAgentCommandHandlerTests
{
    private readonly Mock<IAgentService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly DisableAgentCommandHandler _handler;

    public DisableAgentCommandHandlerTests()
    {
        _handler = new DisableAgentCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_MapsAgentDto()
    {
        var agent = new AgentDefinition
        {
            Id = Guid.NewGuid(),
            Name = "A",
            Status = AgentStatus.Inactive,
            CreatedAt = DateTime.UtcNow
        };
        _service.Setup(s => s.DisableAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Success(agent));

        var result = await _handler.Handle(new DisableAgentCommand(agent.Id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(AgentStatus.Inactive);
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.DisableAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<AgentDefinition>.Failure("Agent not found"));

        var result = await _handler.Handle(new DisableAgentCommand(Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }
}

public class AssignWorkflowCommandHandlerTests
{
    private readonly Mock<IAgentService> _service = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly AssignWorkflowCommandHandler _handler;

    public AssignWorkflowCommandHandlerTests()
    {
        _handler = new AssignWorkflowCommandHandler(_service.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_Success_ReturnsSuccessDefault()
    {
        _service.Setup(s => s.AssignWorkflowAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(new AssignWorkflowCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Failure_ReturnsFailure()
    {
        _service.Setup(s => s.AssignWorkflowAsync(
                It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Failure("Agent not found"));

        var result = await _handler.Handle(new AssignWorkflowCommand(Guid.NewGuid(), Guid.NewGuid()), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent not found");
    }
}
