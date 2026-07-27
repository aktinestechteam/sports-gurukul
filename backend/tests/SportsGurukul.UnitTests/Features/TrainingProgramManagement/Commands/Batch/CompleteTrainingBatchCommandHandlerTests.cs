using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CompleteTrainingBatch;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Batch;

public class CompleteTrainingBatchCommandHandlerTests
{
    private readonly Mock<ILogger<CompleteTrainingBatchCommandHandler>> _loggerMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly CompleteTrainingBatchCommandHandler _handler;

    public CompleteTrainingBatchCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CompleteTrainingBatchCommandHandler>>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();

        _handler = new CompleteTrainingBatchCommandHandler(
            _loggerMock.Object,
            _batchRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BatchIsActive()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);
        var command = new CompleteTrainingBatchCommand(batchId);

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
        var command = new CompleteTrainingBatchCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain($"Training batch with ID {batchId} not found");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchIsNotActive()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Waitlisted);
        var command = new CompleteTrainingBatchCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training batch can only be completed when status is Active");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_SetEndDateToNow_When_Completing()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);
        var command = new CompleteTrainingBatchCommand(batchId);
        TrainingBatch? capturedBatch = null;

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _batchRepositoryMock.Setup(r => r.Update(It.IsAny<TrainingBatch>()))
            .Callback<TrainingBatch>(b => capturedBatch = b);

        var beforeComplete = DateTime.UtcNow;
        await _handler.Handle(command, CancellationToken.None);
        var afterComplete = DateTime.UtcNow;

        capturedBatch.Should().NotBeNull();
        capturedBatch!.EndDate.Should().NotBeNull();
        capturedBatch.EndDate!.Value.Should().BeOnOrAfter(beforeComplete).And.BeOnOrBefore(afterComplete);
        capturedBatch.Status.Should().Be(BatchStatus.Completed);
    }
}
