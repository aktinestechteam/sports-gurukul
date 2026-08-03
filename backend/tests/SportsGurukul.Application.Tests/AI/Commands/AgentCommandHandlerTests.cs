using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Agent;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Tests.AI.Commands;

public class CreateAgentCommandHandlerTests
{
    private readonly Mock<IAgentService> _serviceMock;
    private readonly CreateAgentCommandHandler _handler;

    public CreateAgentCommandHandlerTests()
    {
        _serviceMock = new Mock<IAgentService>();
        _handler = new CreateAgentCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var command = new CreateAgentCommand(
            null, null, "Scout", null, AIAgentType.Researcher, null, 0.7, 5, true, null, null);
        var expected = Result<AgentDto>.Success(new AgentDto(
            Guid.NewGuid(), null, null, "Scout", null, AIAgentType.Researcher, null,
            0.7, 5, true, true, new List<ToolDto>(), DateTime.UtcNow, null));

        _serviceMock.Setup(s => s.CreateAsync(It.IsAny<CreateAgentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Scout");
        _serviceMock.Verify(s => s.CreateAsync(
            It.Is<CreateAgentRequest>(r => r.Name == "Scout"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class AssignWorkflowCommandHandlerTests
{
    private readonly Mock<IAgentService> _serviceMock;
    private readonly AssignWorkflowCommandHandler _handler;

    public AssignWorkflowCommandHandlerTests()
    {
        _serviceMock = new Mock<IAgentService>();
        _handler = new AssignWorkflowCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var agentId = Guid.NewGuid();
        var workflowId = Guid.NewGuid();
        var expected = Result<AgentDto>.Success(new AgentDto(
            agentId, workflowId, null, "Scout", null, AIAgentType.Researcher, null,
            null, null, true, true, new List<ToolDto>(), DateTime.UtcNow, null));

        _serviceMock.Setup(s => s.AssignWorkflowAsync(agentId, workflowId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new AssignWorkflowCommand(agentId, workflowId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.WorkflowId.Should().Be(workflowId);
        _serviceMock.Verify(s => s.AssignWorkflowAsync(agentId, workflowId, It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class EnableAgentCommandHandlerTests
{
    private readonly Mock<IAgentService> _serviceMock;
    private readonly EnableAgentCommandHandler _handler;

    public EnableAgentCommandHandlerTests()
    {
        _serviceMock = new Mock<IAgentService>();
        _handler = new EnableAgentCommandHandler(_serviceMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_DelegatesToService()
    {
        var agentId = Guid.NewGuid();
        var expected = Result<AgentDto>.Success(new AgentDto(
            agentId, null, null, "Scout", null, AIAgentType.Researcher, null,
            null, null, true, true, new List<ToolDto>(), DateTime.UtcNow, null));

        _serviceMock.Setup(s => s.EnableAsync(agentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new EnableAgentCommand(agentId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsActive.Should().BeTrue();
        _serviceMock.Verify(s => s.EnableAsync(agentId, It.IsAny<CancellationToken>()), Times.Once);
    }
}
