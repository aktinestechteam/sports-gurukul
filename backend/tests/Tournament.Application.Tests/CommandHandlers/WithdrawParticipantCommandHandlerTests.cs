using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TournamentManagement.Commands.WithdrawParticipant;
using SportsGurukul.Domain.Enums;

namespace Tournament.Application.Tests.CommandHandlers;

public class WithdrawParticipantCommandHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
    private readonly Mock<IRegistrationRepository> _registrationRepositoryMock = MockRepositoryBuilder.CreateRegistrationRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
    private readonly Mock<ILogger<WithdrawParticipantCommandHandler>> _loggerMock = MockLoggerBuilder.Create<WithdrawParticipantCommandHandler>();
    private readonly WithdrawParticipantCommandHandler _handler;

    public WithdrawParticipantCommandHandlerTests()
    {
        _handler = new WithdrawParticipantCommandHandler(
            _tournamentRepositoryMock.Object,
            _registrationRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.RegistrationOpen);
        var athleteId = Guid.NewGuid();
        var registration = TestDataBuilder.CreateRegistration(
            status: TournamentRegistrationStatus.Pending,
            tournamentId: tournament.Id,
            athleteId: athleteId);
        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        _registrationRepositoryMock.Setup(r => r.GetByTournamentIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRegistration> { registration });

        var result = await _handler.Handle(new WithdrawParticipantCommand
        {
            TournamentId = tournament.Id,
            ParticipantId = athleteId,
            Reason = "Schedule conflict"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        registration.RegistrationStatus.Should().Be(TournamentRegistrationStatus.Cancelled);
        _registrationRepositoryMock.Verify(r => r.Update(registration), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TournamentNotFound_ReturnsFailure()
    {
        var result = await _handler.Handle(new WithdrawParticipantCommand
        {
            TournamentId = Guid.NewGuid(),
            ParticipantId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament not found.");
    }

    [Fact]
    public async Task Handle_InvalidStatus_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.Live);
        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var result = await _handler.Handle(new WithdrawParticipantCommand
        {
            TournamentId = tournament.Id,
            ParticipantId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Participant cannot be withdrawn in current tournament status.");
    }

    [Fact]
    public async Task Handle_RegistrationNotFound_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.RegistrationOpen);
        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        _registrationRepositoryMock.Setup(r => r.GetByTournamentIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TournamentRegistration>());

        var result = await _handler.Handle(new WithdrawParticipantCommand
        {
            TournamentId = tournament.Id,
            ParticipantId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Registration not found for this participant.");
    }
}
