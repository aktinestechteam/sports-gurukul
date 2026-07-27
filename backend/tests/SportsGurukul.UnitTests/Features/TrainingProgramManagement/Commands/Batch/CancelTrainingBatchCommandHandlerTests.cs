using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.CancelTrainingBatch;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Batch;

public class CancelTrainingBatchCommandHandlerTests
{
    private readonly Mock<ILogger<CancelTrainingBatchCommandHandler>> _loggerMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly CancelTrainingBatchCommandHandler _handler;

    public CancelTrainingBatchCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<CancelTrainingBatchCommandHandler>>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();

        _handler = new CancelTrainingBatchCommandHandler(
            _loggerMock.Object,
            _batchRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BatchIsActive()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);
        var command = new CancelTrainingBatchCommand(batchId);

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
        var command = new CancelTrainingBatchCommand(batchId);

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
        var command = new CancelTrainingBatchCommand(batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Training batch can only be cancelled when status is Active");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }
}
