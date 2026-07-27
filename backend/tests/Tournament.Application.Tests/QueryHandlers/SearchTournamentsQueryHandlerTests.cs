using SportsGurukul.Application.Features.TournamentManagement.Queries.SearchTournaments;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;
using TournamentTestShared;

namespace Tournament.Application.Tests.QueryHandlers;

public class SearchTournamentsQueryHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<ILogger<SearchTournamentsQueryHandler>> _loggerMock;
    private readonly SearchTournamentsQueryHandler _handler;

    public SearchTournamentsQueryHandlerTests()
    {
        _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
        _loggerMock = MockLoggerBuilder.Create<SearchTournamentsQueryHandler>();
        _handler = new SearchTournamentsQueryHandler(
            _tournamentRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournaments = new List<TournamentEntity>
        {
            TestDataBuilder.CreateTournament(TournamentStatus.Draft),
            TestDataBuilder.CreateTournament(TournamentStatus.Draft)
        };

        _tournamentRepositoryMock
            .Setup(r => r.SearchAsync(
                It.IsAny<Guid?>(),
                It.IsAny<TournamentStatus?>(),
                It.IsAny<TournamentType?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournaments);

        _tournamentRepositoryMock
            .Setup(r => r.CountSearchAsync(
                It.IsAny<Guid?>(),
                It.IsAny<TournamentStatus?>(),
                It.IsAny<TournamentType?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var query = new SearchTournamentsQuery
        {
            Page = 1,
            PageSize = 20
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().HaveCount(2);
        result.Value.TotalRecords.Should().Be(2);
        result.Value.CurrentPage.Should().Be(1);
        result.Value.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task Handle_NoResults_ReturnsEmptyList()
    {
        _tournamentRepositoryMock
            .Setup(r => r.SearchAsync(
                It.IsAny<Guid?>(),
                It.IsAny<TournamentStatus?>(),
                It.IsAny<TournamentType?>(),
                It.IsAny<string?>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentEntity>());

        _tournamentRepositoryMock
            .Setup(r => r.CountSearchAsync(
                It.IsAny<Guid?>(),
                It.IsAny<TournamentStatus?>(),
                It.IsAny<TournamentType?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var query = new SearchTournamentsQuery
        {
            SearchTerm = "nonexistent",
            Page = 1,
            PageSize = 20
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Items.Should().BeEmpty();
        result.Value.TotalRecords.Should().Be(0);
        result.Value.TotalPages.Should().Be(0);
    }
}
