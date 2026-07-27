using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TournamentManagement.Commands.RegisterParticipant;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Domain.Enums;

namespace Tournament.Application.Tests.CommandHandlers;

public class RegisterParticipantCommandHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
    private readonly Mock<IRegistrationRepository> _registrationRepositoryMock = MockRepositoryBuilder.CreateRegistrationRepository();
    private readonly Mock<IApplicationDbContext> _contextMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
    private readonly Mock<ILogger<RegisterParticipantCommandHandler>> _loggerMock = MockLoggerBuilder.Create<RegisterParticipantCommandHandler>();
    private readonly RegisterParticipantCommandHandler _handler;

    public RegisterParticipantCommandHandlerTests()
    {
        var participants = new List<TournamentParticipant>();
        var mockSet = new Mock<DbSet<TournamentParticipant>>();
        mockSet.Setup(m => m.Add(It.IsAny<TournamentParticipant>()))
            .Callback<TournamentParticipant>(p => participants.Add(p));
        _contextMock.Setup(c => c.TournamentParticipants).Returns(mockSet.Object);

        _handler = new RegisterParticipantCommandHandler(
            _tournamentRepositoryMock.Object,
            _registrationRepositoryMock.Object,
            _contextMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.RegistrationOpen);
        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        _registrationRepositoryMock.Setup(r => r.IsAlreadyRegisteredAsync(
                tournament.Id, It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _registrationRepositoryMock.Setup(r => r.GetRegistrationCountAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);

        var result = await _handler.Handle(new RegisterParticipantCommand
        {
            TournamentId = tournament.Id,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = Guid.NewGuid(),
            RegistrantName = "Test Athlete"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.ParticipantName.Should().Be("Test Athlete");
        _registrationRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TournamentRegistration>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TournamentNotFound_ReturnsFailure()
    {
        var result = await _handler.Handle(new RegisterParticipantCommand
        {
            TournamentId = Guid.NewGuid(),
            ParticipantType = TournamentParticipantType.Athlete,
            RegistrantName = "Test Athlete"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament not found.");
    }

    [Fact]
    public async Task Handle_RegistrationNotOpen_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.Draft);
        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var result = await _handler.Handle(new RegisterParticipantCommand
        {
            TournamentId = tournament.Id,
            ParticipantType = TournamentParticipantType.Athlete,
            RegistrantName = "Test Athlete"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Registration is not open for this tournament.");
    }

    [Fact]
    public async Task Handle_AlreadyRegistered_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.RegistrationOpen);
        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        _registrationRepositoryMock.Setup(r => r.IsAlreadyRegisteredAsync(
                tournament.Id, It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new RegisterParticipantCommand
        {
            TournamentId = tournament.Id,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = Guid.NewGuid(),
            RegistrantName = "Test Athlete"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Participant is already registered for this tournament.");
    }

    [Fact]
    public async Task Handle_MaxParticipantsReached_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.RegistrationOpen, maxParticipants: 2);
        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        _registrationRepositoryMock.Setup(r => r.IsAlreadyRegisteredAsync(
                tournament.Id, It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        _registrationRepositoryMock.Setup(r => r.GetRegistrationCountAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        var result = await _handler.Handle(new RegisterParticipantCommand
        {
            TournamentId = tournament.Id,
            ParticipantType = TournamentParticipantType.Athlete,
            AthleteId = Guid.NewGuid(),
            RegistrantName = "Test Athlete"
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament has reached maximum participants.");
    }
}
