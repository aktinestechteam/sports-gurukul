using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteRanking;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAthleteRankingQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<GetAthleteRankingQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAthleteRankingQueryHandler>();
    private readonly GetAthleteRankingQueryHandler _handler;

    public GetAthleteRankingQueryHandlerTests()
    {
        _handler = new GetAthleteRankingQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new GetAthleteRankingQuery { AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_RankingNotFound_ReturnsFailure()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteRepositoryMock.Setup(r => r.GetAthleteRankingAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SportsGurukul.Domain.Entities.Ranking?)null);

        var result = await _handler.Handle(new GetAthleteRankingQuery { AthleteId = athlete.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Ranking not found for this athlete.");
    }

    [Fact]
    public async Task Handle_RankingExists_ReturnsRanking()
    {
        var athlete = TestDataBuilder.CreateAthleteWithDetails();
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteRepositoryMock.Setup(r => r.GetAthleteRankingAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete.Ranking);

        var result = await _handler.Handle(new GetAthleteRankingQuery { AthleteId = athlete.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.CurrentRank.Should().Be("10");
        result.Value.StateRank.Should().Be("5");
        result.Value.NationalRank.Should().Be("50");
        result.Value.InternationalRank.Should().Be("500");
        result.Value.RankingAuthority.Should().Be("World Athletics");
    }
}
