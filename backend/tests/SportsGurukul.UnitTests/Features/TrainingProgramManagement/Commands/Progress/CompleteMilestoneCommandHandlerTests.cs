using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.CompleteMilestone;
using TrainingProgramEntity = SportsGurukul.Domain.Entities.TrainingProgram;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Progress;

public class CompleteMilestoneCommandHandlerTests
{
    private readonly Mock<ITrainingProgramRepository> _programRepositoryMock;
    private readonly Mock<ILogger<CompleteMilestoneCommandHandler>> _loggerMock;
    private readonly CompleteMilestoneCommandHandler _handler;

    public CompleteMilestoneCommandHandlerTests()
    {
        _programRepositoryMock = new Mock<ITrainingProgramRepository>();
        _loggerMock = new Mock<ILogger<CompleteMilestoneCommandHandler>>();
        _handler = new CompleteMilestoneCommandHandler(
            _programRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCompletion()
    {
        var programId = Guid.NewGuid();
        var milestoneId = Guid.NewGuid();
        var milestone = TestHelpers.CreateTestMilestone(milestoneId, isCompleted: false);
        var program = TestHelpers.CreateTestProgram(programId);
        program.Milestones = new List<SportsGurukul.Domain.Entities.TrainingMilestone> { milestone };

        var command = new CompleteMilestoneCommand { ProgramId = programId, MilestoneId = milestoneId };

        _programRepositoryMock.Setup(r => r.GetByIdAsync(programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        milestone.IsCompleted.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ProgramNotFound()
    {
        var command = new CompleteMilestoneCommand { ProgramId = Guid.NewGuid(), MilestoneId = Guid.NewGuid() };

        _programRepositoryMock.Setup(r => r.GetByIdAsync(command.ProgramId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgramEntity?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Program not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_MilestoneNotFound()
    {
        var programId = Guid.NewGuid();
        var program = TestHelpers.CreateTestProgram(programId);
        program.Milestones = new List<SportsGurukul.Domain.Entities.TrainingMilestone>();

        var command = new CompleteMilestoneCommand { ProgramId = programId, MilestoneId = Guid.NewGuid() };

        _programRepositoryMock.Setup(r => r.GetByIdAsync(programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Milestone not found in the specified program");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AlreadyCompleted()
    {
        var programId = Guid.NewGuid();
        var milestoneId = Guid.NewGuid();
        var milestone = TestHelpers.CreateTestMilestone(milestoneId, isCompleted: true);
        var program = TestHelpers.CreateTestProgram(programId);
        program.Milestones = new List<SportsGurukul.Domain.Entities.TrainingMilestone> { milestone };

        var command = new CompleteMilestoneCommand { ProgramId = programId, MilestoneId = milestoneId };

        _programRepositoryMock.Setup(r => r.GetByIdAsync(programId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(program);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Milestone is already completed");
    }
}
