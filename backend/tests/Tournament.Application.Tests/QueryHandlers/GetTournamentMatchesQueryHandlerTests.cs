using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentMatches;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;
using SportsGurukul.Domain.Entities;
using TournamentTestShared;

namespace Tournament.Application.Tests.QueryHandlers;

public class GetTournamentMatchesQueryHandlerTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock;
    private readonly Mock<ILogger<GetTournamentMatchesQueryHandler>> _loggerMock;
    private readonly GetTournamentMatchesQueryHandler _handler;

    public GetTournamentMatchesQueryHandlerTests()
    {
        _matchRepositoryMock = MockRepositoryBuilder.CreateMatchRepository();
        _loggerMock = MockLoggerBuilder.Create<GetTournamentMatchesQueryHandler>();
        _handler = new GetTournamentMatchesQueryHandler(
            _matchRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_GetAll_ReturnsSuccess()
    {
        var tournamentId = Guid.NewGuid();
        var matches = new List<TournamentMatch>
        {
            TestDataBuilder.CreateMatch(MatchStatus.Scheduled, tournamentId),
            TestDataBuilder.CreateMatch(MatchStatus.InProgress, tournamentId)
        };

        _matchRepositoryMock
            .Setup(r => r.GetByTournamentIdAsync(tournamentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matches);

        var query = new GetTournamentMatchesQuery
        {
            TournamentId = tournamentId
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_FilterByStatus_ReturnsSuccess()
    {
        var tournamentId = Guid.NewGuid();
        var matches = new List<TournamentMatch>
        {
            TestDataBuilder.CreateMatch(MatchStatus.Completed, tournamentId)
        };

        _matchRepositoryMock
            .Setup(r => r.GetByStatusAsync(tournamentId, MatchStatus.Completed, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matches);

        var query = new GetTournamentMatchesQuery
        {
            TournamentId = tournamentId,
            Status = MatchStatus.Completed
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
    }

    [Fact]
    public async Task Handle_FilterByRoundId_ReturnsSuccess()
    {
        var tournamentId = Guid.NewGuid();
        var roundId = Guid.NewGuid();
        var matches = new List<TournamentMatch>
        {
            TestDataBuilder.CreateMatch(MatchStatus.Scheduled, tournamentId)
        };

        _matchRepositoryMock
            .Setup(r => r.GetByRoundIdAsync(roundId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(matches);

        var query = new GetTournamentMatchesQuery
        {
            TournamentId = tournamentId,
            RoundId = roundId
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
    }
}
