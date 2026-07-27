using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.TransferEnrollment;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Enrollment;

public class TransferEnrollmentCommandHandlerTests
{
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ILogger<TransferEnrollmentCommandHandler>> _loggerMock;
    private readonly TransferEnrollmentCommandHandler _handler;

    public TransferEnrollmentCommandHandlerTests()
    {
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _loggerMock = new Mock<ILogger<TransferEnrollmentCommandHandler>>();

        _handler = new TransferEnrollmentCommandHandler(
            _batchRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static TransferEnrollmentCommand CreateValidCommand(
        Guid? enrollmentId = null,
        Guid? sourceBatchId = null,
        Guid? targetBatchId = null) => new()
    {
        EnrollmentId = enrollmentId ?? Guid.NewGuid(),
        SourceBatchId = sourceBatchId ?? Guid.NewGuid(),
        TargetBatchId = targetBatchId ?? Guid.NewGuid()
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidTransfer()
    {
        var enrollmentId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var sourceBatchId = Guid.NewGuid();
        var targetBatchId = Guid.NewGuid();

        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: sourceBatchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var sourceBatch = TestHelpers.CreateTestBatch(id: sourceBatchId, status: BatchStatus.Active);
        sourceBatch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var targetBatch = TestHelpers.CreateTestBatch(id: targetBatchId, status: BatchStatus.Active);
        targetBatch.Enrollments = new List<TrainingEnrollment>();

        var command = CreateValidCommand(enrollmentId, sourceBatchId, targetBatchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(sourceBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBatch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(targetBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetBatch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.BatchId.Should().Be(targetBatchId);
        _batchRepositoryMock.Verify(r => r.Update(sourceBatch), Times.Once);
        _batchRepositoryMock.Verify(r => r.Update(targetBatch), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SameSourceAndTarget()
    {
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(sourceBatchId: batchId, targetBatchId: batchId);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Source and target batch cannot be the same");
        _batchRepositoryMock.Verify(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SourceBatchNotFound()
    {
        var command = CreateValidCommand();

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.SourceBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Source batch not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentNotFoundInSource()
    {
        var sourceBatchId = Guid.NewGuid();
        var sourceBatch = TestHelpers.CreateTestBatch(id: sourceBatchId, status: BatchStatus.Active);
        sourceBatch.Enrollments = new List<TrainingEnrollment>();

        var command = CreateValidCommand(sourceBatchId: sourceBatchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(sourceBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBatch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Active enrollment not found in source batch");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TargetBatchNotFound()
    {
        var enrollmentId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var sourceBatchId = Guid.NewGuid();
        var targetBatchId = Guid.NewGuid();

        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: sourceBatchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var sourceBatch = TestHelpers.CreateTestBatch(id: sourceBatchId, status: BatchStatus.Active);
        sourceBatch.Enrollments = new List<TrainingEnrollment> { enrollment };

        var command = CreateValidCommand(enrollmentId, sourceBatchId, targetBatchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(sourceBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBatch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(targetBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Target batch not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TargetBatchIsNotActive()
    {
        var enrollmentId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var sourceBatchId = Guid.NewGuid();
        var targetBatchId = Guid.NewGuid();

        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: sourceBatchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var sourceBatch = TestHelpers.CreateTestBatch(id: sourceBatchId, status: BatchStatus.Active);
        sourceBatch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var targetBatch = TestHelpers.CreateTestBatch(id: targetBatchId, status: BatchStatus.Inactive);
        targetBatch.Enrollments = new List<TrainingEnrollment>();

        var command = CreateValidCommand(enrollmentId, sourceBatchId, targetBatchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(sourceBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBatch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(targetBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetBatch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Target batch is not active");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AthleteAlreadyInTargetBatch()
    {
        var enrollmentId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var sourceBatchId = Guid.NewGuid();
        var targetBatchId = Guid.NewGuid();

        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: sourceBatchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var sourceBatch = TestHelpers.CreateTestBatch(id: sourceBatchId, status: BatchStatus.Active);
        sourceBatch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var targetBatch = TestHelpers.CreateTestBatch(id: targetBatchId, status: BatchStatus.Active);
        targetBatch.Enrollments = new List<TrainingEnrollment>
        {
            TestHelpers.CreateTestEnrollment(batchId: targetBatchId, athleteId: athleteId, status: EnrollmentStatus.Active)
        };

        var command = CreateValidCommand(enrollmentId, sourceBatchId, targetBatchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(sourceBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBatch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(targetBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetBatch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete is already enrolled in the target batch");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_TargetBatchIsFull()
    {
        var enrollmentId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var sourceBatchId = Guid.NewGuid();
        var targetBatchId = Guid.NewGuid();

        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: sourceBatchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var sourceBatch = TestHelpers.CreateTestBatch(id: sourceBatchId, status: BatchStatus.Active);
        sourceBatch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var targetBatch = TestHelpers.CreateTestBatch(id: targetBatchId, status: BatchStatus.Active, maximumSeats: 1);
        targetBatch.Enrollments = new List<TrainingEnrollment>
        {
            TestHelpers.CreateTestEnrollment(batchId: targetBatchId, athleteId: Guid.NewGuid(), status: EnrollmentStatus.Active)
        };

        var command = CreateValidCommand(enrollmentId, sourceBatchId, targetBatchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(sourceBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sourceBatch);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(targetBatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(targetBatch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Target batch has reached maximum capacity");
    }
}
