using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.RemoveAthlete;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class RemoveAthleteCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IRepository<CoachAthlete>> _coachAthleteRepositoryMock = TestMocks.CreateCoachAthleteRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<RemoveAthleteCommandHandler>> _loggerMock = TestMocks.CreateLogger<RemoveAthleteCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly RemoveAthleteCommandHandler _handler;

    public RemoveAthleteCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new RemoveAthleteCommandHandler(
            _coachRepositoryMock.Object,
            _coachAthleteRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new RemoveAthleteCommand
        {
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_AssignmentNotFound_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachAthleteRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachAthlete, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachAthlete>());

        var result = await _handler.Handle(new RemoveAthleteCommand
        {
            CoachId = coachId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete assignment not found.");
    }

    [Fact]
    public async Task Handle_ValidAssignment_SetsInactiveAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var coachAthlete = TestDataBuilder.CreateCoachAthlete(coachId, athleteId);
        coachAthlete.IsActive = true;

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachAthleteRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachAthlete, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachAthlete> { coachAthlete });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RemoveAthleteCommand
        {
            CoachId = coachId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        coachAthlete.IsActive.Should().BeFalse();
        _coachAthleteRepositoryMock.Verify(r => r.Update(coachAthlete), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
