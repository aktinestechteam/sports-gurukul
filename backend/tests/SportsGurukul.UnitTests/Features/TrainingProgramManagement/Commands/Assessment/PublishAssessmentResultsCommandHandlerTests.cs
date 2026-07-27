using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.PublishAssessmentResults;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Assessment;

public class PublishAssessmentResultsCommandHandlerTests
{
    private readonly Mock<IAssessmentRepository> _assessmentRepositoryMock;
    private readonly Mock<ILogger<PublishAssessmentResultsCommandHandler>> _loggerMock;
    private readonly PublishAssessmentResultsCommandHandler _handler;

    public PublishAssessmentResultsCommandHandlerTests()
    {
        _assessmentRepositoryMock = new Mock<IAssessmentRepository>();
        _loggerMock = new Mock<ILogger<PublishAssessmentResultsCommandHandler>>();
        _handler = new PublishAssessmentResultsCommandHandler(
            _assessmentRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ResultsExist()
    {
        var assessmentId = Guid.NewGuid();
        var assessment = TestHelpers.CreateTestAssessment(assessmentId);
        var results = new List<AssessmentResult>
        {
            new() { Id = Guid.NewGuid(), AssessmentId = assessmentId, AthleteId = Guid.NewGuid(), Score = 80, IsPassed = true },
            new() { Id = Guid.NewGuid(), AssessmentId = assessmentId, AthleteId = Guid.NewGuid(), Score = 40, IsPassed = false }
        };

        var command = new PublishAssessmentResultsCommand { AssessmentId = assessmentId };

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _assessmentRepositoryMock.Setup(r => r.GetResultsByAssessmentIdAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(results);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_AssessmentNotFound()
    {
        var assessmentId = Guid.NewGuid();
        var command = new PublishAssessmentResultsCommand { AssessmentId = assessmentId };

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingAssessment?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Assessment not found");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_NoResults()
    {
        var assessmentId = Guid.NewGuid();
        var assessment = TestHelpers.CreateTestAssessment(assessmentId);

        var command = new PublishAssessmentResultsCommand { AssessmentId = assessmentId };

        _assessmentRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(assessment);
        _assessmentRepositoryMock.Setup(r => r.GetResultsByAssessmentIdAsync(assessmentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AssessmentResult>());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("No results found");
    }
}
