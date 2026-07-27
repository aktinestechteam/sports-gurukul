using SportsGurukul.Application.Features.TournamentManagement.Commands.UpdateTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using TournamentTestShared;

namespace Tournament.Application.Tests.CommandHandlers;

public class UpdateTournamentCommandHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<UpdateTournamentCommandHandler>> _loggerMock;
    private readonly UpdateTournamentCommandHandler _handler;

    public UpdateTournamentCommandHandlerTests()
    {
        _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
        _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
        _loggerMock = MockLoggerBuilder.Create<UpdateTournamentCommandHandler>();
        _handler = new UpdateTournamentCommandHandler(
            _tournamentRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournament = TestDataBuilder.CreateTournament(TournamentStatus.Draft);
        _tournamentRepositoryMock
            .Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var command = new UpdateTournamentCommand
        {
            TournamentId = tournament.Id,
            TournamentName = "Updated Name",
            Venue = "Updated Venue"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TournamentName.Should().Be("Updated Name");
        _tournamentRepositoryMock.Verify(r => r.Update(It.IsAny<TournamentEntity>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TournamentNotFound_ReturnsFailure()
    {
        var command = new UpdateTournamentCommand
        {
            TournamentId = Guid.NewGuid(),
            TournamentName = "Updated Name"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament not found.");
    }

    [Fact]
    public async Task Handle_TournamentNotDraft_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(TournamentStatus.Published);
        _tournamentRepositoryMock
            .Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var command = new UpdateTournamentCommand
        {
            TournamentId = tournament.Id,
            TournamentName = "Updated Name"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament can only be updated in Draft status.");
    }
}
