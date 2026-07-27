using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.AssignCoachToBatch;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Batch;

public class AssignCoachToBatchCommandHandlerTests
{
    private readonly Mock<ILogger<AssignCoachToBatchCommandHandler>> _loggerMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ICoachRepository> _coachRepositoryMock;
    private readonly AssignCoachToBatchCommandHandler _handler;

    public AssignCoachToBatchCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<AssignCoachToBatchCommandHandler>>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _coachRepositoryMock = new Mock<ICoachRepository>();

        _handler = new AssignCoachToBatchCommandHandler(
            _loggerMock.Object,
            _batchRepositoryMock.Object,
            _coachRepositoryMock.Object);
    }

    private static AssignCoachToBatchCommand CreateValidCommand(Guid? batchId = null, Guid? coachId = null) => new(
        Id: batchId ?? Guid.NewGuid(),
        CoachId: coachId ?? Guid.NewGuid());

    private void SetupValidDependencies(AssignCoachToBatchCommand command, TrainingBatch batch)
    {
        var coach = TestHelpers.CreateTestCoach(id: command.CoachId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCommand()
    {
        var batchId = Guid.NewGuid();
        var coachId = Guid.NewGuid();
        var command = CreateValidCommand(batchId, coachId);
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);

        SetupValidDependencies(command, batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchNotFound()
    {
        var command = CreateValidCommand();

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Training batch with ID {command.Id} not found");
        _coachRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchIsNotActive()
    {
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(batchId);
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Waitlisted);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach can only be reassigned to Active training batches");
        _coachRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_CoachNotFound()
    {
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(batchId);
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(command.CoachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Coach with ID {command.CoachId} not found");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateCoachId_When_Assigning()
    {
        var batchId = Guid.NewGuid();
        var newCoachId = Guid.NewGuid();
        var command = CreateValidCommand(batchId, newCoachId);
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);

        SetupValidDependencies(command, batch);

        await _handler.Handle(command, CancellationToken.None);

        _batchRepositoryMock.Verify(r => r.Update(It.Is<TrainingBatch>(b =>
            b.CoachId == newCoachId)), Times.Once);
    }
}
