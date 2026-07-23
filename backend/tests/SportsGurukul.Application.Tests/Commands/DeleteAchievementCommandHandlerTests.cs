using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAchievement;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class DeleteAchievementCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<AthleteAchievement>> _athleteAchievementRepositoryMock = TestMocks.CreateAthleteAchievementRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<DeleteAchievementCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeleteAchievementCommandHandler>();
    private readonly DeleteAchievementCommandHandler _handler;

    public DeleteAchievementCommandHandlerTests()
    {
        _handler = new DeleteAchievementCommandHandler(
            _athleteRepositoryMock.Object,
            _athleteAchievementRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new DeleteAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            AchievementId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_AchievementNotLinked_ReturnsFailure()
    {
        var athleteId = Guid.NewGuid();
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateAthlete(id: athleteId));
        _athleteAchievementRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAchievement, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteAchievement>());

        var result = await _handler.Handle(new DeleteAchievementCommand
        {
            AthleteId = athleteId,
            AchievementId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Achievement not found for this athlete.");
    }

    [Fact]
    public async Task Handle_ValidDelete_RemovesAndReturnsSuccess()
    {
        var athleteId = Guid.NewGuid();
        var achievementId = Guid.NewGuid();
        var athleteAchievement = new AthleteAchievement
        {
            Id = Guid.NewGuid(),
            AthleteId = athleteId,
            AchievementId = achievementId
        };

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateAthlete(id: athleteId));
        _athleteAchievementRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAchievement, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteAchievement> { athleteAchievement });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteAchievementCommand
        {
            AthleteId = athleteId,
            AchievementId = achievementId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _athleteAchievementRepositoryMock.Verify(r => r.Remove(athleteAchievement), Times.Once);
    }
}
