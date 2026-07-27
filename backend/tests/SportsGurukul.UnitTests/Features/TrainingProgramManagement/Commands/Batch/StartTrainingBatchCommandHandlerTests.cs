using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.StartTrainingBatch;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Batch;

public class StartTrainingBatchCommandHandlerTests
{
    private readonly Mock<ILogger<StartTrainingBatchCommandHandler>> _loggerMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly StartTrainingBatchCommandHandler _handler;

    public StartTrainingBatchCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<StartTrainingBatchCommandHandler>>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();

        _handler = new StartTrainingBatchCommandHandler(
            _loggerMock.Object,
            _batchRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BatchIsWaitlisted()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Waitlisted);
        var command = new StartTrainingBatchCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BatchIsInactive()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Inactive);
        var command = new StartTrainingBatchCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchNotFound()
    {
        var batchId = Guid.NewGuid();
        var command = new StartTrainingBatchCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Training batch with ID {batchId} not found");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchIsAlreadyActive()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);
        var command = new StartTrainingBatchCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training batch can only be started when status is Waitlisted or Inactive");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchIsCompleted()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Completed);
        var command = new StartTrainingBatchCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training batch can only be started when status is Waitlisted or Inactive");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }
}
