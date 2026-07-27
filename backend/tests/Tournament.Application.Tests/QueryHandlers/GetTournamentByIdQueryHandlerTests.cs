using SportsGurukul.Application.Features.TournamentManagement.Queries.GetTournamentById;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Entities;
using TournamentTestShared;

namespace Tournament.Application.Tests.QueryHandlers;

public class GetTournamentByIdQueryHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<ILogger<GetTournamentByIdQueryHandler>> _loggerMock;
    private readonly GetTournamentByIdQueryHandler _handler;

    public GetTournamentByIdQueryHandlerTests()
    {
        _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
        _loggerMock = MockLoggerBuilder.Create<GetTournamentByIdQueryHandler>();
        _handler = new GetTournamentByIdQueryHandler(
            _tournamentRepositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournament = TestDataBuilder.CreateTournament();
        tournament.Registrations = new List<TournamentRegistration>
        {
            TestDataBuilder.CreateRegistration(tournamentId: tournament.Id),
            TestDataBuilder.CreateRegistration(tournamentId: tournament.Id)
        };

        _tournamentRepositoryMock
            .Setup(r => r.GetWithDetailsAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var query = new GetTournamentByIdQuery { TournamentId = tournament.Id };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TournamentName.Should().Be("Test Tournament");
        result.Value.RegisteredCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_TournamentNotFound_ReturnsFailure()
    {
        var tournamentId = Guid.NewGuid();

        _tournamentRepositoryMock
            .Setup(r => r.GetWithDetailsAsync(tournamentId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TournamentEntity?)null);

        var query = new GetTournamentByIdQuery { TournamentId = tournamentId };

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament not found.");
    }
}
