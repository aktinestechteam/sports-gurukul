using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.Commands.Workflow;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace AI.Application.Tests.Commands.Workflow;

public class CreateWorkflowCommandHandlerTests
{
    private readonly Mock<IWorkflowDefinitionRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly CreateWorkflowCommandHandler _handler;

    public CreateWorkflowCommandHandlerTests()
    {
        _handler = new CreateWorkflowCommandHandler(_repo.Object, _unitOfWork.Object);
    }

    [Fact]
    public async Task Handle_AddsWorkflowAndMapsDto()
    {
        var id = Guid.NewGuid();
        _repo.Setup(r => r.AddAsync(
                It.IsAny<WorkflowDefinition>(), It.IsAny<CancellationToken>()))
            .Callback<WorkflowDefinition, CancellationToken>((e, _) => e.Id = id)
            .ReturnsAsync((WorkflowDefinition e, CancellationToken _) => e);

        var result = await _handler.Handle(
            new CreateWorkflowCommand("Flow", "desc", "steps", "triggers", "conds", "vars"),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var dto = result.Value!;
        dto.Id.Should().Be(id);
        dto.Name.Should().Be("Flow");
        dto.Description.Should().Be("desc");
        dto.Steps.Should().Be("steps");
        dto.Triggers.Should().Be("triggers");
        dto.Conditions.Should().Be("conds");
        dto.Variables.Should().Be("vars");
        dto.Status.Should().Be(WorkflowStatus.Draft);
        dto.Version.Should().Be(1);

        _repo.Verify(r => r.AddAsync(
            It.Is<WorkflowDefinition>(w =>
                w.Name == "Flow" && w.Status == WorkflowStatus.Draft && w.Version == 1),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}

public class UpdateWorkflowCommandHandlerTests
{
    private readonly Mock<IWorkflowDefinitionRepository> _repo = new();
    private readonly Mock<IUnitOfWork> _unitOfWork = new();
    private readonly UpdateWorkflowCommandHandler _handler;

    public UpdateWorkflowCommandHandlerTests()
    {
        _handler = new UpdateWorkflowCommandHandler(_repo.Object, _unitOfWork.Object);
    }

    private static WorkflowDefinition BuildWorkflow() => new()
    {
        Id = Guid.NewGuid(),
        Name = "Flow",
        Status = WorkflowStatus.Active,
        Version = 3,
        CreatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task Handle_ExistingWorkflow_UpdatesFieldsAndIncrementsVersion()
    {
        var workflow = BuildWorkflow();
        _repo.Setup(r => r.GetByIdAsync(workflow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workflow);

        var result = await _handler.Handle(
            new UpdateWorkflowCommand(workflow.Id, "New Name", null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("New Name");
        result.Value.Version.Should().Be(4);
        _repo.Verify(r => r.Update(It.Is<WorkflowDefinition>(w =>
            w.Name == "New Name" && w.Version == 4)), Times.Once);
    }

    [Fact]
    public async Task Handle_NullFields_KeepsExistingValues()
    {
        var workflow = BuildWorkflow();
        _repo.Setup(r => r.GetByIdAsync(workflow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workflow);

        var result = await _handler.Handle(
            new UpdateWorkflowCommand(workflow.Id, null, null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Flow");
        result.Value.Version.Should().Be(4);
    }

    [Fact]
    public async Task Handle_WorkflowNotFound_ReturnsFailure()
    {
        _repo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((WorkflowDefinition?)null);

        var result = await _handler.Handle(
            new UpdateWorkflowCommand(Guid.NewGuid(), "x", null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Workflow not found");
    }

    [Fact]
    public async Task Handle_DeletedWorkflow_ReturnsFailure()
    {
        var workflow = BuildWorkflow();
        workflow.IsDeleted = true;
        _repo.Setup(r => r.GetByIdAsync(workflow.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(workflow);

        var result = await _handler.Handle(
            new UpdateWorkflowCommand(workflow.Id, "x", null, null, null, null, null),
            CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Workflow not found");
    }
}
