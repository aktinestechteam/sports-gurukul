using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.TournamentManagement.Commands.GenerateFixtures;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using SportsGurukul.Application.Features.TournamentManagement.Services;
using SportsGurukul.Domain.Enums;

namespace Tournament.Application.Tests.CommandHandlers;

public class GenerateFixturesCommandHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
    private readonly Mock<IFixtureGenerationService> _fixtureGenerationServiceMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
    private readonly Mock<ILogger<GenerateFixturesCommandHandler>> _loggerMock = MockLoggerBuilder.Create<GenerateFixturesCommandHandler>();
    private readonly GenerateFixturesCommandHandler _handler;

    public GenerateFixturesCommandHandlerTests()
    {
        _handler = new GenerateFixturesCommandHandler(
            _tournamentRepositoryMock.Object,
            _fixtureGenerationServiceMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.RegistrationClosed);
        var participants = new List<TournamentParticipant>
        {
            TestDataBuilder.CreateParticipant(isActive: true, tournamentId: tournament.Id),
            TestDataBuilder.CreateParticipant(isActive: true, tournamentId: tournament.Id)
        };
        var stages = new List<TournamentStage>
        {
            TestDataBuilder.CreateStage(tournamentId: tournament.Id)
        };
        tournament.Participants = participants;
        tournament.Stages = stages;

        var fixtures = new List<TournamentFixture>
        {
            TestDataBuilder.CreateFixture(tournamentId: tournament.Id)
        };

        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        _tournamentRepositoryMock.Setup(r => r.GetWithDetailsAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        _fixtureGenerationServiceMock.Setup(s => s.GenerateFixturesAsync(
                tournament, It.IsAny<IReadOnlyList<TournamentParticipant>>(),
                It.IsAny<IReadOnlyList<TournamentStage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixtures);

        var result = await _handler.Handle(new GenerateFixturesCommand
        {
            TournamentId = tournament.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
        tournament.Status.Should().Be(TournamentStatus.FixtureGeneration);
        _tournamentRepositoryMock.Verify(r => r.Update(tournament), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_TournamentNotFound_ReturnsFailure()
    {
        var result = await _handler.Handle(new GenerateFixturesCommand
        {
            TournamentId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Tournament not found.");
    }

    [Fact]
    public async Task Handle_WrongStatus_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.RegistrationOpen);
        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var result = await _handler.Handle(new GenerateFixturesCommand
        {
            TournamentId = tournament.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Fixtures can only be generated after registration closes.");
    }

    [Fact]
    public async Task Handle_NoParticipants_ReturnsFailure()
    {
        var tournament = TestDataBuilder.CreateTournament(status: TournamentStatus.RegistrationClosed);
        tournament.Participants = new List<TournamentParticipant>();
        tournament.Stages = new List<TournamentStage>();

        _tournamentRepositoryMock.Setup(r => r.GetByIdAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);
        _tournamentRepositoryMock.Setup(r => r.GetWithDetailsAsync(tournament.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tournament);

        var result = await _handler.Handle(new GenerateFixturesCommand
        {
            TournamentId = tournament.Id
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("No active participants found.");
    }
}
