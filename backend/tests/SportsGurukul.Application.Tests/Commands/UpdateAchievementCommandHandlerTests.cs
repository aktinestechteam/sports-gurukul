using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.UpdateAchievement;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateAchievementCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<Achievement>> _achievementRepositoryMock = TestMocks.CreateAchievementRepository();
    private readonly Mock<IRepository<AthleteAchievement>> _athleteAchievementRepositoryMock = TestMocks.CreateAthleteAchievementRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateAchievementCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateAchievementCommandHandler>();
    private readonly UpdateAchievementCommandHandler _handler;

    public UpdateAchievementCommandHandlerTests()
    {
        _handler = new UpdateAchievementCommandHandler(
            _athleteRepositoryMock.Object,
            _achievementRepositoryMock.Object,
            _athleteAchievementRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new UpdateAchievementCommand
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
        var achievementId = Guid.NewGuid();
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateAthlete(id: athleteId));
        _athleteAchievementRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAchievement, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteAchievement>());

        var result = await _handler.Handle(new UpdateAchievementCommand
        {
            AthleteId = athleteId,
            AchievementId = achievementId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Achievement not found for this athlete.");
    }

    [Fact]
    public async Task Handle_AchievementRecordNotFound_ReturnsFailure()
    {
        var athleteId = Guid.NewGuid();
        var achievementId = Guid.NewGuid();
        var athleteAchievement = new AthleteAchievement
        {
            Id = Guid.NewGuid(),
            AthleteId = athleteId,
            AchievementId = achievementId,
            AwardedDate = DateTime.UtcNow.AddDays(-10),
            Notes = "Some notes"
        };

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateAthlete(id: athleteId));
        _athleteAchievementRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAchievement, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteAchievement> { athleteAchievement });
        _achievementRepositoryMock.Setup(r => r.GetByIdAsync(achievementId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Achievement?)null);

        var result = await _handler.Handle(new UpdateAchievementCommand
        {
            AthleteId = athleteId,
            AchievementId = achievementId,
            Title = "New Title"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Achievement record not found.");
    }

    [Fact]
    public async Task Handle_PartialUpdate_OnlyNonNullFieldsUpdated()
    {
        var athleteId = Guid.NewGuid();
        var achievementId = Guid.NewGuid();
        var achievement = TestDataBuilder.CreateAchievement(id: achievementId);
        achievement.Title = "Original Title";
        achievement.Competition = "Original Competition";
        var athleteAchievement = new AthleteAchievement
        {
            Id = Guid.NewGuid(),
            AthleteId = athleteId,
            AchievementId = achievementId,
            AwardedDate = DateTime.UtcNow.AddDays(-10),
            Notes = "Original notes"
        };

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateAthlete(id: athleteId));
        _athleteAchievementRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAchievement, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteAchievement> { athleteAchievement });
        _achievementRepositoryMock.Setup(r => r.GetByIdAsync(achievementId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(achievement);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateAchievementCommand
        {
            AthleteId = athleteId,
            AchievementId = achievementId,
            Title = "Only Title Changed"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Only Title Changed");
        result.Value.Competition.Should().Be("Original Competition");
        result.Value.Notes.Should().Be("Original notes");
    }

    [Fact]
    public async Task Handle_ValidUpdate_UpdatesAndReturnsSuccess()
    {
        var athleteId = Guid.NewGuid();
        var achievementId = Guid.NewGuid();
        var achievement = TestDataBuilder.CreateAchievement(id: achievementId);
        var athleteAchievement = new AthleteAchievement
        {
            Id = Guid.NewGuid(),
            AthleteId = athleteId,
            AchievementId = achievementId,
            AwardedDate = DateTime.UtcNow.AddDays(-10),
            Notes = "Original notes"
        };

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateAthlete(id: athleteId));
        _athleteAchievementRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<AthleteAchievement, bool>>>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteAchievement> { athleteAchievement });
        _achievementRepositoryMock.Setup(r => r.GetByIdAsync(achievementId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(achievement);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateAchievementCommand
        {
            AthleteId = athleteId,
            AchievementId = achievementId,
            Title = "Updated Title",
            Competition = "Updated Competition",
            Position = "2nd",
            Level = AchievementLevel.National,
            Date = DateTime.UtcNow.AddDays(-30),
            CertificateUrl = "https://example.com/new-cert.pdf",
            Notes = "Updated notes"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("Updated Title");
        result.Value.Competition.Should().Be("Updated Competition");
        result.Value.Position.Should().Be("2nd");
        result.Value.Level.Should().Be("National");
        result.Value.Notes.Should().Be("Updated notes");
        _achievementRepositoryMock.Verify(r => r.Update(achievement), Times.Once);
        _athleteAchievementRepositoryMock.Verify(r => r.Update(athleteAchievement), Times.Once);
    }
}
