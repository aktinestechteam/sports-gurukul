using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentRankings;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using TournamentTestShared;

namespace Tournament.Application.Tests.QueryHandlers;

public class GetTournamentRankingsQueryHandlerTests
{
    private readonly Mock<IRankingRepository> _rankingRepositoryMock;
    private readonly Mock<ILogger<GetTournamentRankingsQueryHandler>> _loggerMock;
    private readonly GetTournamentRankingsQueryHandler _handler;

    public GetTournamentRankingsQueryHandlerTests()
    {
        _rankingRepositoryMock = MockRepositoryBuilder.CreateRankingRepository();
        _loggerMock = MockLoggerBuilder.Create<GetTournamentRankingsQueryHandler>();
        _handler = new GetTournamentRankingsQueryHandler(
            _rankingRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_GetAll_ReturnsSuccess()
    {
        var tournamentId = Guid.NewGuid();
        var rankings = new List<TournamentRanking>
        {
            TestDataBuilder.CreateRanking(1, tournamentId),
            TestDataBuilder.CreateRanking(2, tournamentId)
        };

        _rankingRepositoryMock
            .Setup(r => r.GetByTournamentIdAsync(tournamentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rankings);

        var query = new GetTournamentRankingsQuery
        {
            TournamentId = tournamentId
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_FilterByCategory_ReturnsSuccess()
    {
        var tournamentId = Guid.NewGuid();
        var categoryId = Guid.NewGuid();
        var rankings = new List<TournamentRanking>
        {
            TestDataBuilder.CreateRanking(1, tournamentId)
        };

        _rankingRepositoryMock
            .Setup(r => r.GetByCategoryIdAsync(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(rankings);

        var query = new GetTournamentRankingsQuery
        {
            TournamentId = tournamentId,
            CategoryId = categoryId
        };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
    }
}
