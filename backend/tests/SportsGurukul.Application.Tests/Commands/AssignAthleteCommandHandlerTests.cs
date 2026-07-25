using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.AssignAthlete;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class AssignAthleteCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<CoachAthlete>> _coachAthleteRepositoryMock = TestMocks.CreateCoachAthleteRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<AssignAthleteCommandHandler>> _loggerMock = TestMocks.CreateLogger<AssignAthleteCommandHandler>();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly AssignAthleteCommandHandler _handler;

    public AssignAthleteCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new AssignAthleteCommandHandler(
            _coachRepositoryMock.Object,
            _athleteRepositoryMock.Object,
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

        var result = await _handler.Handle(new AssignAthleteCommand
        {
            CoachId = Guid.NewGuid(),
            AthleteId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_CoachNotActive_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        coach.Status = CoachStatus.Inactive;

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new AssignAthleteCommand
        {
            CoachId = coachId,
            AthleteId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach must be active to assign athletes.");
    }

    [Fact]
    public async Task Handle_CoachNotVerified_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        coach.VerificationStatus = VerificationStatus.Pending;

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new AssignAthleteCommand
        {
            CoachId = coachId,
            AthleteId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach must be verified to assign athletes.");
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new AssignAthleteCommand
        {
            CoachId = coachId,
            AthleteId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_AthleteNotActive_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var athlete = TestDataBuilder.CreateAthlete(id: athleteId);
        athlete.Status = AthleteStatus.Inactive;

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(new AssignAthleteCommand
        {
            CoachId = coachId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete must be active to be assigned.");
    }

    [Fact]
    public async Task Handle_DuplicateAssignment_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var athlete = TestDataBuilder.CreateAthlete(id: athleteId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _coachAthleteRepositoryMock.Setup(r => r.AnyAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachAthlete, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new AssignAthleteCommand
        {
            CoachId = coachId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete is already assigned to this coach.");
    }

    [Fact]
    public async Task Handle_ValidAssignment_AssignsAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var athleteId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var athlete = TestDataBuilder.CreateAthlete(id: athleteId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _athleteRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _coachAthleteRepositoryMock.Setup(r => r.AnyAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<CoachAthlete, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _coachAthleteRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachAthlete>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachAthlete());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AssignAthleteCommand
        {
            CoachId = coachId,
            AthleteId = athleteId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.AthleteId.Should().Be(athleteId);
        result.Value.FullName.Should().Be(athlete.User.FullName);
        result.Value.AthleteCode.Should().Be(athlete.AthleteCode);
        _coachAthleteRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachAthlete>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
