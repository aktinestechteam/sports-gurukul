using SportsGurukul.Application.Features.TournamentManagement.Commands.PublishTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using TournamentTestShared;

namespace Tournament.Application.Tests.CommandHandlers;

public class PublishTournamentCommandHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<PublishTournamentCommandHandler>> _loggerMock;
    private readonly PublishTournamentCommandHandler _handler;

    public PublishTournamentCommandHandlerTests()
    {
        _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
        _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
        _loggerMock = MockLoggerBuilder.Create<PublishTournamentCommandHandler>();
        _handler = new PublishTournamentCommandHandler(
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

        var command = new PublishTournamentCommand { TournamentId = tournament.Id };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(TournamentStatus.Published);
        result.Value.IsPublished.Should().BeTrue();
        _tournamentRepositoryMock.Verify(r => r.Update(It.IsAny<TournamentEntity>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TournamentNotFound_ReturnsFailure()
    {
        var command = new PublishTournamentCommand { TournamentId = Guid.NewGuid() };

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

        var command = new PublishTournamentCommand { TournamentId = tournament.Id };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only draft tournaments can be published.");
    }
}
