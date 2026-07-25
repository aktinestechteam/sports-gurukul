using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Queries.GetCoachSports;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Queries;

public class GetCoachSportsQueryHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<ILogger<GetCoachSportsQueryHandler>> _loggerMock = TestMocks.CreateLogger<GetCoachSportsQueryHandler>();
    private readonly GetCoachSportsQueryHandler _handler;

    public GetCoachSportsQueryHandlerTests()
    {
        _handler = new GetCoachSportsQueryHandler(
            _coachRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsCoachSports()
    {
        var coachId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var coachSports = new List<CoachSport>
        {
            TestDataBuilder.CreateCoachSport(coachId, sportId)
        };

        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coachSports);

        var result = await _handler.Handle(new GetCoachSportsQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value![0].Name.Should().Be("Cricket");
        result.Value[0].IsPrimarySport.Should().BeFalse();
    }

    [Fact]
    public async Task Handle_NoSports_ReturnsEmptyList()
    {
        var coachId = Guid.NewGuid();
        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachSport>());

        var result = await _handler.Handle(new GetCoachSportsQuery { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }
}
