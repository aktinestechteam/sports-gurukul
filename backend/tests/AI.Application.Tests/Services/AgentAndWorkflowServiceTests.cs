using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Features.AIManagement.DomainEvents;
using SportsGurukul.Application.Features.AIManagement.Services;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Services;

public class AgentServiceTests
{
    private readonly Mock<IAgentDefinitionRepository> _agentRepo = new();
    private readonly Mock<IWorkflowDefinitionRepository> _workflowRepo = new();
    private readonly Mock<IPublisher> _publisher = new();
    private readonly AgentService _service;

    public AgentServiceTests()
    {
        _service = new AgentService(
            _agentRepo.Object, _workflowRepo.Object, _publisher.Object,
            NullLogger<AgentService>.Instance);
    }

    [Fact]
    public async Task CreateAsync_CreatesDraftAgentWithDefaultsAndPublishesEvent()
    {
        _publisher.Setup(p => p.Publish(It.IsAny<AgentCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(
            new CreateAgentRequest("Analyst", "desc", null, null, null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var agent = result.Value!;
        agent.Name.Should().Be("Analyst");
        agent.Status.Should().Be(AgentStatus.Draft);
        agent.MaxIterations.Should().Be(10);
        agent.RequiresApproval.Should().BeFalse();

        _agentRepo.Verify(r => r.AddAsync(It.IsAny<AgentDefinition>(), It.IsAny<CancellationToken>()), Times.Once);
        _publisher.Verify(p => p.Publish(It.IsAny<AgentCreatedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_UsesProvidedMaxIterationsAndApproval()
    {
        _publisher.Setup(p => p.Publish(It.IsAny<AgentCreatedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var result = await _service.CreateAsync(
            new CreateAgentRequest("Analyst", null, null, null, null, null, null, 5, true),
            CancellationToken.None);

        result.Value!.MaxIterations.Should().Be(5);
        result.Value.RequiresApproval.Should().BeTrue();
    }

    [Fact]
    public async Task UpdateAsync_UpdatesNonNullFields()
    {
        var agent = new AgentDefinition { Id = Guid.NewGuid(), Name = "Old", MaxIterations = 1, RequiresApproval = false };
        _agentRepo.Setup(r => r.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);

        var result = await _service.UpdateAsync(
            new UpdateAgentRequest(agent.Id, "New", null, null, null, "rules", null, 9, true),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        agent.Name.Should().Be("New");
        agent.Rules.Should().Be("rules");
        agent.MaxIterations.Should().Be(9);
        agent.RequiresApproval.Should().BeTrue();
        agent.Description.Should().BeNull();
        _agentRepo.Verify(r => r.Update(agent), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NotFound_ReturnsFailure()
    {
        _agentRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AgentDefinition?)null);

        var result = await _service.UpdateAsync(
            new UpdateAgentRequest(Guid.NewGuid(), "x", null, null, null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent not found");
    }

    [Fact]
    public async Task EnableAsync_SetsActive()
    {
        var agent = new AgentDefinition { Id = Guid.NewGuid(), Status = AgentStatus.Draft };
        _agentRepo.Setup(r => r.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);

        var result = await _service.EnableAsync(agent.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        agent.Status.Should().Be(AgentStatus.Active);
    }

    [Fact]
    public async Task DisableAsync_SetsInactive()
    {
        var agent = new AgentDefinition { Id = Guid.NewGuid(), Status = AgentStatus.Active };
        _agentRepo.Setup(r => r.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);

        var result = await _service.DisableAsync(agent.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        agent.Status.Should().Be(AgentStatus.Inactive);
    }

    [Fact]
    public async Task AssignWorkflowAsync_Success_PublishesEvent()
    {
        _publisher.Setup(p => p.Publish(It.IsAny<WorkflowAssignedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        var agent = new AgentDefinition { Id = Guid.NewGuid() };
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid() };
        _agentRepo.Setup(r => r.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);
        _workflowRepo.Setup(r => r.GetByIdAsync(workflow.Id, It.IsAny<CancellationToken>())).ReturnsAsync(workflow);

        var result = await _service.AssignWorkflowAsync(agent.Id, workflow.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _publisher.Verify(p => p.Publish(It.Is<WorkflowAssignedEvent>(e =>
            e.AgentId == agent.Id && e.WorkflowDefinitionId == workflow.Id), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AssignWorkflowAsync_AgentNotFound_ReturnsFailure()
    {
        _agentRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AgentDefinition?)null);

        var result = await _service.AssignWorkflowAsync(Guid.NewGuid(), Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Agent not found");
    }

    [Fact]
    public async Task AssignWorkflowAsync_WorkflowNotFound_ReturnsFailure()
    {
        var agent = new AgentDefinition { Id = Guid.NewGuid() };
        _agentRepo.Setup(r => r.GetByIdAsync(agent.Id, It.IsAny<CancellationToken>())).ReturnsAsync(agent);
        _workflowRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkflowDefinition?)null);

        var result = await _service.AssignWorkflowAsync(agent.Id, Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Workflow not found");
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsFailure()
    {
        _agentRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AgentDefinition?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task SearchAsync_ReturnsResults()
    {
        _agentRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<AgentDefinition, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AgentDefinition> { new() { Id = Guid.NewGuid() } });

        var result = await _service.SearchAsync(
            new SearchAgentsRequest(null, null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }
}

public class WorkflowServiceTests
{
    private readonly Mock<IWorkflowDefinitionRepository> _workflowRepo = new();
    private readonly WorkflowService _service;

    public WorkflowServiceTests()
    {
        _service = new WorkflowService(_workflowRepo.Object, NullLogger<WorkflowService>.Instance);
    }

    [Fact]
    public async Task GetByIdAsync_Success_ReturnsWorkflow()
    {
        var workflow = new WorkflowDefinition { Id = Guid.NewGuid() };
        _workflowRepo.Setup(r => r.GetByIdWithDetailsAsync(workflow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workflow);

        var result = await _service.GetByIdAsync(workflow.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(workflow.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NotFound_ReturnsFailure()
    {
        _workflowRepo.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkflowDefinition?)null);

        var result = await _service.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Workflow not found");
    }

    [Fact]
    public async Task SearchAsync_ReturnsResults()
    {
        _workflowRepo.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<WorkflowDefinition, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<WorkflowDefinition> { new() { Id = Guid.NewGuid() } });

        var result = await _service.SearchAsync(new SearchWorkflowsRequest(null, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Should().HaveCount(1);
    }
}
