using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.SubmitAssessmentResult;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Assessment;

public class SubmitAssessmentResultCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _assessmentRepositoryMock;
    private readonly Mock<ITrainingBatchRepository> _batchRepositoryMock;
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock;
    private readonly Mock<ILogger<SubmitAssessmentResultCommandHandler>> _loggerMock;
    private readonly SubmitAssessmentResultCommandHandler _handler;

    public SubmitAssessmentResultCommandHandlerTests()
    {
        _assessmentRepositoryMock = new Mock<IAssessmentRepository>();
        _batchRepositoryMock = new Mock<ITrainingBatchRepository>();
        _athleteRepositoryMock = new Mock<IAthleteRepository>();
        _loggerMock = new Mock<ILogger<SubmitAssessmentResultCommandHandler>>();
        _handler = new SubmitAssessmentResultCommandHandler(
            _assessmentRepositoryMock.Object,
            _batchRepositoryMock.Object,
            _athleteRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static SubmitAssessmentResultCommand CreateValidCommand(
        Guid? assessmentId = null,
        Guid? athleteId = null,
        decimal score = 75) => new()
    {
        AssessmentId = assessmentId ?? Guid.NewGuid(),
        AthleteId = athleteId ?? Guid.NewGuid(),
        Score = score,
        Remarks = "Good performance"
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidSubmission()
    {
        var sessionId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();

        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var assessment = TestHelpers.CreateTestAssessment(sessionId: sessionId);
        assessment.Session = session;
        var athlete = TestHelpers.CreateTestAthlete(athleteId);
        var enrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };

        var command = CreateValidCommand(assessmentId: assessment.Id, athleteId: athleteId, score: 75);

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Score.Should().Be(75);
        result.Value.AthleteId.Should().Be(athleteId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AssessmentNotFound()
    {
        var command = CreateValidCommand();

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(command.AssessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingAssessment?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assessment not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AthleteNotFound()
    {
        var sessionId = Guid.NewGuid();
        var assessment = TestHelpers.CreateTestAssessment(sessionId: sessionId);
        var command = CreateValidCommand(assessmentId: assessment.Id);

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(command.AthleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AthleteNotEnrolled()
    {
        var sessionId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var otherAthleteId = Guid.NewGuid();

        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var assessment = TestHelpers.CreateTestAssessment(sessionId: sessionId);
        assessment.Session = session;
        var athlete = TestHelpers.CreateTestAthlete(athleteId);
        var enrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: otherAthleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };

        var command = CreateValidCommand(assessmentId: assessment.Id, athleteId: athleteId);

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete is not enrolled in the associated batch");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ResultAlreadyExists()
    {
        var sessionId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();

        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var assessment = TestHelpers.CreateTestAssessment(sessionId: sessionId);
        assessment.Session = session;
        var athlete = TestHelpers.CreateTestAthlete(athleteId);
        var enrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };

        assessment.Results = new List<AssessmentResult>
        {
            new() { AthleteId = athleteId, Score = 60 }
        };

        var command = CreateValidCommand(assessmentId: assessment.Id, athleteId: athleteId);

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("A result already exists for this athlete on this assessment");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_ScoreExceedsMaximum()
    {
        var sessionId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();

        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var assessment = TestHelpers.CreateTestAssessment(sessionId: sessionId);
        assessment.Session = session;
        assessment.MaximumScore = 100;
        var athlete = TestHelpers.CreateTestAthlete(athleteId);
        var enrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };

        var command = CreateValidCommand(assessmentId: assessment.Id, athleteId: athleteId, score: 150);

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("Score must be between 0 and");
    }

    [Fact]
    public async Task Handle_Should_SetIsPassedTrue_When_AbovePassingScore()
    {
        var sessionId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();

        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var assessment = TestHelpers.CreateTestAssessment(sessionId: sessionId);
        assessment.Session = session;
        assessment.PassingScore = 50;
        var athlete = TestHelpers.CreateTestAthlete(athleteId);
        var enrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };

        var command = CreateValidCommand(assessmentId: assessment.Id, athleteId: athleteId, score: 75);

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsPassed.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_SetIsPassedFalse_When_BelowPassingScore()
    {
        var sessionId = Guid.NewGuid();
        var batchId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();

        var session = TestHelpers.CreateTestSession(id: sessionId, batchId: batchId);
        var assessment = TestHelpers.CreateTestAssessment(sessionId: sessionId);
        assessment.Session = session;
        assessment.PassingScore = 50;
        var athlete = TestHelpers.CreateTestAthlete(athleteId);
        var enrollment = TestHelpers.CreateTestEnrollment(batchId: batchId, athleteId: athleteId, status: EnrollmentStatus.Active);
        var batch = TestHelpers.CreateTestBatch(id: batchId);
        batch.Enrollments = new List<TrainingEnrollment> { enrollment };

        var command = CreateValidCommand(assessmentId: assessment.Id, athleteId: athleteId, score: 30);

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessment.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _batchRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(batchId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(batch);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.IsPassed.Should().BeFalse();
    }
}
