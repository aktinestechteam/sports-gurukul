using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.AdvancedSearchAthletes;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Application.Tests.Common;

namespace SportsGurukul.Application.Tests.Queries;

public class AdvancedSearchAthletesQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<AdvancedSearchAthletesQueryHandler>> _loggerMock = TestMocks.CreateLogger<AdvancedSearchAthletesQueryHandler>();
    private readonly AdvancedSearchAthletesQueryHandler _handler;

    public AdvancedSearchAthletesQueryHandlerTests()
    {
        _handler = new AdvancedSearchAthletesQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ReturnsPaginatedResults()
    {
        var athletes = new List<AthleteSummaryDto>
        {
            new() { Id = Guid.NewGuid(), FullName = "Test Athlete" }
        };
        _athleteRepositoryMock.Setup(r => r.SearchAthletesAsync(
            It.IsAny<AthleteSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((athletes, 1));

        var result = await _handler.Handle(new AdvancedSearchAthletesQuery
        {
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_FirstPage_SetsNextCursor()
    {
        var athletes = Enumerable.Range(1, 20).Select(i =>
            new AthleteSummaryDto { Id = Guid.NewGuid(), FullName = $"Athlete {i}" }).ToList();
        _athleteRepositoryMock.Setup(r => r.SearchAthletesAsync(
            It.IsAny<AthleteSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((athletes, 50));

        var result = await _handler.Handle(new AdvancedSearchAthletesQuery
        {
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NextCursor.Should().NotBeNull();
        result.Value.PreviousCursor.Should().BeNull();
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsNoCursor()
    {
        _athleteRepositoryMock.Setup(r => r.SearchAthletesAsync(
            It.IsAny<AthleteSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<AthleteSummaryDto>(), 0));

        var result = await _handler.Handle(new AdvancedSearchAthletesQuery
        {
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.NextCursor.Should().BeNull();
        result.Value.PreviousCursor.Should().BeNull();
    }
}
