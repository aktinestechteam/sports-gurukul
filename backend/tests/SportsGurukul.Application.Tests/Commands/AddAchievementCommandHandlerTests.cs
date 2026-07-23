using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AddAchievement;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class AddAchievementCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<Achievement>> _achievementRepositoryMock = TestMocks.CreateAchievementRepository();
    private readonly Mock<IRepository<AthleteAchievement>> _athleteAchievementRepositoryMock = TestMocks.CreateAthleteAchievementRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<AddAchievementCommandHandler>> _loggerMock = TestMocks.CreateLogger<AddAchievementCommandHandler>();
    private readonly AddAchievementCommandHandler _handler;

    public AddAchievementCommandHandlerTests()
    {
        _handler = new AddAchievementCommandHandler(
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

        var result = await _handler.Handle(new AddAchievementCommand
        {
            AthleteId = Guid.NewGuid(),
            Title = "State Championship",
            Level = AchievementLevel.State,
            Date = DateTime.UtcNow.AddDays(-30)
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_ValidAchievement_CreatesAndReturnsSuccess()
    {
        var athleteId = Guid.NewGuid();
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateAthlete(id: athleteId));
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AddAchievementCommand
        {
            AthleteId = athleteId,
            Title = "National Championship",
            Competition = "National Level Cricket",
            Position = "Winner",
            Level = AchievementLevel.National,
            Date = DateTime.UtcNow.AddDays(-60),
            CertificateUrl = "https://example.com/cert.pdf",
            Notes = "Great performance"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Title.Should().Be("National Championship");
        result.Value.Competition.Should().Be("National Level Cricket");
        result.Value.Position.Should().Be("Winner");
        result.Value.Level.Should().Be("National");
        result.Value.CertificateUrl.Should().Be("https://example.com/cert.pdf");
        result.Value.Notes.Should().Be("Great performance");
        _achievementRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Achievement>(), It.IsAny<CancellationToken>()), Times.Once);
        _athleteAchievementRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AthleteAchievement>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
