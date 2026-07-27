using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Enrollment.CompleteEnrollment;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Enrollment;

public class CompleteEnrollmentCommandHandlerTests
{
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ITrainingProgressRepository> _progressRepositoryMock;
    private readonly Mock<ILogger<CompleteEnrollmentCommandHandler>> _loggerMock;
    private readonly CompleteEnrollmentCommandHandler _handler;

    public CompleteEnrollmentCommandHandlerTests()
    {
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _progressRepositoryMock = new Mock<ITrainingProgressRepository>();
        _loggerMock = new Mock<ILogger<CompleteEnrollmentCommandHandler>>();

        _handler = new CompleteEnrollmentCommandHandler(
            _batchRepositoryMock.Object,
            _progressRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static CompleteEnrollmentCommand CreateValidCommand(Guid? enrollmentId = null, Guid? batchId = null) => new()
    {
        EnrollmentId = enrollmentId ?? Guid.NewGuid(),
        BatchId = batchId ?? Guid.NewGuid()
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCompletion()
    {
        var batchId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var command = CreateValidCommand(enrollmentId, batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _progressRepositoryMock.Setup(r => r.GetByEnrollmentIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgress?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Status.Should().Be("Completed");
        _batchRepositoryMock.Verify(r => r.Update(batch), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_BatchNotFound()
    {
        var command = CreateValidCommand();

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.BatchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingBatch?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Batch not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentNotFound()
    {
        var batchId = Guid.NewGuid();
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment>();
        var command = CreateValidCommand(batchId: batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Enrollment not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentIsNotActive()
    {
        var batchId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: batchId, status: EnrollmentStatus.Completed);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var command = CreateValidCommand(enrollmentId, batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only active enrollments can be completed");
    }

    [Fact]
    public async Task Handle_Should_CreateNewProgress_When_NoExistingProgress()
    {
        var batchId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var command = CreateValidCommand(enrollmentId, batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _progressRepositoryMock.Setup(r => r.GetByEnrollmentIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgress?)null);
        _progressRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingProgress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgress p, CancellationToken _) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Progress.Should().NotBeNull();
        result.Value.Progress.CompletedPercentage.Should().Be(100);
        result.Value.Progress.CurrentLevel.Should().Be("Completed");
        _progressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingProgress>(), It.IsAny<CancellationToken>()), Times.Once);
        _progressRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingProgress>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateExistingProgress_When_Exists()
    {
        var batchId = Guid.NewGuid();
        var enrollmentId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };
        var existingProgress = TestHelpers.CreateTestProgress(enrollmentId: enrollmentId);
        enrollment.Progress = existingProgress;
        var command = CreateValidCommand(enrollmentId, batchId);

        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
        _progressRepositoryMock.Setup(r => r.GetByEnrollmentIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProgress);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existingProgress.CompletedPercentage.Should().Be(100);
        existingProgress.CurrentLevel.Should().Be("Completed");
        _progressRepositoryMock.Verify(r => r.Update(existingProgress), Times.Once);
        _progressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingProgress>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
