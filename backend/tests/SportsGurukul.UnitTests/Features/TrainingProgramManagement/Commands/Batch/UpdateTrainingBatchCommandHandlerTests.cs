using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Batch.UpdateTrainingBatch;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Batch;

public class UpdateTrainingBatchCommandHandlerTests
{
    private readonly Mock<ILogger<UpdateTrainingBatchCommandHandler>> _loggerMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly UpdateTrainingBatchCommandHandler _handler;

    public UpdateTrainingBatchCommandHandlerTests()
    {
        _loggerMock = new Mock<ILogger<UpdateTrainingBatchCommandHandler>>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();

        _handler = new UpdateTrainingBatchCommandHandler(
            _loggerMock.Object,
            _batchRepositoryMock.Object);
    }

    private static UpdateTrainingBatchCommand CreateValidCommand(Guid? batchId = null) => new(
        Id: batchId ?? Guid.NewGuid(),
        StartDate: DateTime.UtcNow.AddDays(1),
        EndDate: DateTime.UtcNow.AddDays(90),
        MaximumSeats: 25);

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_BatchIsActive()
    {
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(batchId);
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);
        var updatedBatch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedBatch);

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
        result.Error.Should().Be("Training batch can only be updated when status is Active");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_StartDateAfterEndDate()
    {
        var batchId = Guid.NewGuid();
        var command = new UpdateTrainingBatchCommand(
            Id: batchId,
            StartDate: DateTime.UtcNow.AddDays(90),
            EndDate: DateTime.UtcNow.AddDays(1),
            MaximumSeats: 25);
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Start date must be before end date");
        _batchRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingBatch>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateFields_When_ValidCommand()
    {
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(batchId);
        var batch = TestHelpers.CreateTestBatch(id: batchId, status: BatchStatus.Active);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        await _handler.Handle(command, CancellationToken.None);

        _batchRepositoryMock.Verify(r => r.Update(It.Is<TrainingBatch>(b =>
            b.StartDate == command.StartDate &&
            b.EndDate == command.EndDate &&
            b.MaximumSeats == command.MaximumSeats)), Times.Once);
    }
}
