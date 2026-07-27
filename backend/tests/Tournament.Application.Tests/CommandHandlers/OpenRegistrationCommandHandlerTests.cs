using SportsGurukul.Application.Features.TournamentManagement.Commands.OpenRegistration;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using TournamentTestShared;

namespace Tournament.Application.Tests.CommandHandlers;

public class OpenRegistrationCommandHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<OpenRegistrationCommandHandler>> _loggerMock;
    private readonly OpenRegistrationCommandHandler _handler;

    public OpenRegistrationCommandHandlerTests()
    {
        _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
        _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
        _loggerMock = MockLoggerBuilder.Create<OpenRegistrationCommandHandler>();
        _handler = new OpenRegistrationCommandHandler(
            _tournamentRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournament = TestDataBuilder.CreateTournament(TournamentStatus.Published);
        _tournamentRepositoryMock
            .Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var command = new OpenRegistrationCommand { TournamentId = tournament.Id };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be(TournamentStatus.RegistrationOpen);
        _tournamentRepositoryMock.Verify(r => r.Update(It.IsAny<TournamentEntity>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TournamentNotFound_ReturnsFailure()
    {
        var command = new OpenRegistrationCommand { TournamentId = Guid.NewGuid() };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament not found.");
    }

    [Fact]
    public async Task Handle_TournamentNotPublished_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(TournamentStatus.Draft);
        _tournamentRepositoryMock
            .Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var command = new OpenRegistrationCommand { TournamentId = tournament.Id };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Registration can only be opened for published tournaments.");
    }
}
