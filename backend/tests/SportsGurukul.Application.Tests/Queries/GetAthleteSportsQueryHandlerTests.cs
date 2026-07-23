using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.GetAthleteSports;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetAthleteSportsQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<GetAthleteSportsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetAthleteSportsQueryHandler>();
    private readonly GetAthleteSportsQueryHandler _handler;

    public GetAthleteSportsQueryHandlerTests()
    {
        _handler = new GetAthleteSportsQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new GetAthleteSportsQuery { AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_AthleteExists_ReturnsSports()
    {
        var athleteId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthlete(id: athleteId);
        var athleteSports = new List<AthleteSport>
        {
            TestDataBuilder.CreateAthleteSport(athleteId),
            TestDataBuilder.CreateAthleteSport(athleteId)
        };

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _athleteRepositoryMock.Setup(r => r.GetAthleteSportsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athleteSports);

        var result = await _handler.Handle(new GetAthleteSportsQuery { AthleteId = athleteId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }
}
