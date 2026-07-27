using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TrainingProgramManagement.Commands.Assessment.CreateAssessment;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.UnitTests.Features.TrainingProgramManagement.Commands.Assessment;

public class CreateAssessmentCommandHandlerTests
{
    private readonly Mock<ISessionRepository> _sessionRepositoryMock;
    private readonly Mock<IAssessmentRepository> _assessmentRepositoryMock;
    private readonly Mock<ILogger<CreateAssessmentCommandHandler>> _loggerMock;
    private readonly CreateAssessmentCommandHandler _handler;

    public CreateAssessmentCommandHandlerTests()
    {
        _sessionRepositoryMock = new Mock<ISessionRepository>();
        _assessmentRepositoryMock = new Mock<IAssessmentRepository>();
        _loggerMock = new Mock<ILogger<CreateAssessmentCommandHandler>>();
        _handler = new CreateAssessmentCommandHandler(
            _sessionRepositoryMock.Object,
            _assessmentRepositoryMock.Object,
            _loggerMock.Object);
    }

    private static CreateAssessmentCommand CreateValidCommand(Guid? sessionId = null) => new()
    {
        SessionId = sessionId ?? Guid.NewGuid(),
        AssessmentType = "SkillTest",
        AssessmentName = "Mid-term Assessment",
        MaximumScore = 100,
        PassingScore = 50
    };

    [Fact]
    public async Task Handle_Should_ReturnSuccess_When_ValidCommand()
    {
        var session = TestHelpers.CreateTestSession();
        var command = CreateValidCommand(sessionId: session.Id);

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(command.SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);
        _assessmentRepositoryMock.Setup(r => r.AddAsync(It.IsAny<TrainingAssessment>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingAssessment a, CancellationToken _) => a);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.AssessmentName.Should().Be(command.AssessmentName);
        result.Value.MaximumScore.Should().Be(command.MaximumScore);
        result.Value.PassingScore.Should().Be(command.PassingScore);
        result.Value.SessionId.Should().Be(command.SessionId);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_SessionNotFound()
    {
        var command = CreateValidCommand();

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(command.SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TrainingSession?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Session not found");
        _assessmentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TrainingAssessment>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_MaximumScoreZeroOrNegative()
    {
        var session = TestHelpers.CreateTestSession();
        var command = CreateValidCommand(sessionId: session.Id) with { MaximumScore = 0 };

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(command.SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Maximum score must be greater than zero");
    }

    [Fact]
    public async Task Handle_Should_ReturnFailure_When_PassingScoreInvalid()
    {
        var session = TestHelpers.CreateTestSession();
        var command = CreateValidCommand(sessionId: session.Id) with { MaximumScore = 100, PassingScore = 150 };

        _sessionRepositoryMock.Setup(r => r.GetByIdAsync(command.SessionId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(session);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Passing score must be between 0 and the maximum score");
    }
}
