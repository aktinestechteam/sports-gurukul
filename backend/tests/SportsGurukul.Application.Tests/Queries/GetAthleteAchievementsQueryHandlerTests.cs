using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteAchievements;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAthleteAchievementsQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<GetAthleteAchievementsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAthleteAchievementsQueryHandler>();
    private readonly GetAthleteAchievementsQueryHandler _handler;

    public GetAthleteAchievementsQueryHandlerTests()
    {
        _handler = new GetAthleteAchievementsQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new GetAthleteAchievementsQuery { AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_AthleteExists_ReturnsAchievements()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteRepositoryMock.Setup(r => r.GetAthleteAchievementsAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete.AthleteAchievements.ToList());

        var result = await _handler.Handle(new GetAthleteAchievementsQuery { AthleteId = athlete.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value!.First().Title.Should().Be("State Championship");
    }
}
