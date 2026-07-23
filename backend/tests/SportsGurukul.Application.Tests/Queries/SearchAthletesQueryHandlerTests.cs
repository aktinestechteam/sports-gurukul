using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Queries.SearchAthletes;
using SportsGurukul.Application.Features.AthleteManagement.DTOs;
using SportsGurukul.Application.Tests.Common;

namespace SportsGurukul.Application.Tests.Queries;

public class SearchAthletesQueryHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ILogger<SearchAthletesQueryHandler>> _loggerMock = TestMocks.CreateLogger<SearchAthletesQueryHandler>();
    private readonly SearchAthletesQueryHandler _handler;

    public SearchAthletesQueryHandlerTests()
    {
        _handler = new SearchAthletesQueryHandler(_athleteRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_NoFilters_ReturnsPaginatedResults()
    {
        var athletes = new List<AthleteSummaryDto>
        {
            new() { Id = Guid.NewGuid(), FullName = "Athlete 1", AthleteCode = "ATH-001" },
            new() { Id = Guid.NewGuid(), FullName = "Athlete 2", AthleteCode = "ATH-002" }
        };
        _athleteRepositoryMock.Setup(r => r.SearchAthletesAsync(
            It.IsAny<AthleteSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((athletes, 2));

        var result = await _handler.Handle(new SearchAthletesQuery
        {
            Page = 1,
            PageSize = 20
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalRecords.Should().Be(2);
        result.Value.TotalPages.Should().Be(1);
        result.Value.CurrentPage.Should().Be(1);
    }

    [Fact]
    public async Task Handle_WithFilters_PassesFiltersToRepository()
    {
        _athleteRepositoryMock.Setup(r => r.SearchAthletesAsync(
            It.IsAny<AthleteSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<AthleteSummaryDto>(), 0));

        await _handler.Handle(new SearchAthletesQuery
        {
            SearchTerm = "test",
            Name = "John",
            SportName = "Cricket",
            City = "Mumbai",
            Page = 2,
            PageSize = 10
        }, CancellationToken.None);

        _athleteRepositoryMock.Verify(r => r.SearchAthletesAsync(
            It.Is<AthleteSearchRequest>(sr =>
                sr.SearchTerm == "test" &&
                sr.Name == "John" &&
                sr.SportName == "Cricket" &&
                sr.City == "Mumbai" &&
                sr.Page == 2 &&
                sr.PageSize == 10),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EmptyResults_ReturnsEmptyResponse()
    {
        _athleteRepositoryMock.Setup(r => r.SearchAthletesAsync(
            It.IsAny<AthleteSearchRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<AthleteSummaryDto>(), 0));

        var result = await _handler.Handle(new SearchAthletesQuery
        {
            SearchTerm = "nonexistent"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
        result.Value.TotalPages.Should().Be(0);
    }
}
