using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Progress.UpdateTrainingProgress;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Progress;

public class UpdateTrainingProgressCommandHandlerTests
{
    private readonly Mock<ITrainingProgressRepository> _progressRepositoryMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<ILogger<UpdateTrainingProgressCommandHandler>> _loggerMock;
    private readonly UpdateTrainingProgressCommandHandler _handler;

    public UpdateTrainingProgressCommandHandlerTests()
    {
        _progressRepositoryMock = new Mock<ITrainingProgressRepository>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _loggerMock = new Mock<ILogger<UpdateTrainingProgressCommandHandler>>();
        _handler = new UpdateTrainingProgressCommandHandler(
            _progressRepositoryMock.Object,
            _batchRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static UpdateTrainingProgressCommand CreateValidCommand(Guid? enrollmentId = null) => new()
    {
        EnrollmentId = enrollmentId ?? Guid.NewGuid(),
        CurrentLevel = "Intermediate",
        CompletedPercentage = 65,
        OverallRating = 4.0m
    };

    private void SetupBatchWithEnrollment(Guid batchId, Guid enrollmentId, EnrollmentStatus status = EnrollmentStatus.Active)
    {
        var enrollment = TestHelpers.CreateTestEnrollment(id: enrollmentId, batchId: batchId, status: status);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };

        _batchRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingBatch> { batch });
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidUpdate()
    {
        var enrollmentId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        SetupBatchWithEnrollment(batchId, enrollmentId);
        _progressRepositoryMock.Setup(r => r.GetByEnrollmentIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgress?)null);
        _progressRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingProgress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgress p, CancellationToken _) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.CurrentLevel.Should().Be(command.CurrentLevel);
        result.Value.CompletedPercentage.Should().Be(command.CompletedPercentage);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentNotFound()
    {
        var enrollmentId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        _batchRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TrainingBatch>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Enrollment not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_EnrollmentNotValidStatus()
    {
        var enrollmentId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        SetupBatchWithEnrollment(batchId, enrollmentId, EnrollmentStatus.Withdrawn);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Enrollment is not in a valid state for progress update");
    }

    [Fact]
    public async Task Handle_Should_CreateNewProgress_When_NoExisting()
    {
        var enrollmentId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        SetupBatchWithEnrollment(batchId, enrollmentId);
        _progressRepositoryMock.Setup(r => r.GetByEnrollmentIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgress?)null);
        _progressRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingProgress>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingProgress p, CancellationToken _) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _progressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingProgress>(), It.IsAny<CancellationToken>()), Times.Once);
        _progressRepositoryMock.Verify(r => r.Update(It.IsAny<TrainingProgress>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_UpdateExistingProgress_When_Exists()
    {
        var enrollmentId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var command = CreateValidCommand(enrollmentId);

        var existingProgress = TestHelpers.CreateTestProgress(enrollmentId: enrollmentId);
        existingProgress.CurrentLevel = "Beginner";
        existingProgress.CompletedPercentage = 30;

        SetupBatchWithEnrollment(batchId, enrollmentId);
        _progressRepositoryMock.Setup(r => r.GetByEnrollmentIdAsync(enrollmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingProgress);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _progressRepositoryMock.Verify(r => r.Update(existingProgress), Times.Once);
        _progressRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingProgress>(), It.IsAny<CancellationToken>()), Times.Never);
        existingProgress.CurrentLevel.Should().Be(command.CurrentLevel);
        existingProgress.CompletedPercentage.Should().Be(command.CompletedPercentage);
    }
}
