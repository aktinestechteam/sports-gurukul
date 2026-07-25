using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.RemoveSport;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class CoachRemoveSportCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IRepository<CoachSport>> _coachSportRepositoryMock = TestMocks.CreateCoachSportRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<RemoveSportCommandHandler>> _loggerMock = TestMocks.CreateLogger<RemoveSportCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly RemoveSportCommandHandler _handler;

    public CoachRemoveSportCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new RemoveSportCommandHandler(
            _coachRepositoryMock.Object,
            _coachSportRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_SportNotAssigned_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachSport>());

        var result = await _handler.Handle(new RemoveSportCommand
        {
            CoachId = coachId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sport is not assigned to the coach.");
    }

    [Fact]
    public async Task Handle_SportAssigned_RemovesAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var coachSport = TestDataBuilder.CreateCoachSport(coachId, sportId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachSport> { coachSport });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RemoveSportCommand
        {
            CoachId = coachId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _coachSportRepositoryMock.Verify(r => r.Remove(coachSport), Times.Once);
    }

    [Fact]
    public async Task Handle_SportExistsButSoftDeleted_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var deletedSport = TestDataBuilder.CreateCoachSport(coachId, sportId);
        deletedSport.IsDeleted = true;

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachSport> { deletedSport });

        var result = await _handler.Handle(new RemoveSportCommand
        {
            CoachId = coachId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sport is not assigned to the coach.");
    }

    [Fact]
    public async Task Handle_SportExistsButDifferentId_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var otherSportId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var coachSport = TestDataBuilder.CreateCoachSport(coachId, otherSportId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachSport> { coachSport });

        var result = await _handler.Handle(new RemoveSportCommand
        {
            CoachId = coachId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sport is not assigned to the coach.");
    }
}
