using SportsGurukul.Application.Features.TournamentManagement.Commands.CloseRegistration;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using TournamentTestShared;

namespace Tournament.Application.Tests.CommandHandlers;

public class CloseRegistrationCommandHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CloseRegistrationCommandHandler>> _loggerMock;
    private readonly CloseRegistrationCommandHandler _handler;

    public CloseRegistrationCommandHandlerTests()
    {
        _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
        _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
        _loggerMock = MockLoggerBuilder.Create<CloseRegistrationCommandHandler>();
        _handler = new CloseRegistrationCommandHandler(
            _tournamentRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournament = TestDataBuilder.CreateTournament(TournamentStatus.RegistrationOpen);
        _tournamentRepositoryMock
            .Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var command = new CloseRegistrationCommand { TournamentId = tournament.Id };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(TournamentStatus.RegistrationClosed);
        _tournamentRepositoryMock.Verify(r => r.Update(It.IsAny<TournamentEntity>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TournamentNotFound_ReturnsFailure()
    {
        var command = new CloseRegistrationCommand { TournamentId = Guid.NewGuid() };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament not found.");
    }

    [Fact]
    public async Task Handle_TournamentNotRegistrationOpen_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(TournamentStatus.Published);
        _tournamentRepositoryMock
            .Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var command = new CloseRegistrationCommand { TournamentId = tournament.Id };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Registration is not currently open for this tournament.");
    }
}
