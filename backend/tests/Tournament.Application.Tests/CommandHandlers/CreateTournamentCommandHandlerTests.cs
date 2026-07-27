using SportsGurukul.Application.Features.TournamentManagement.Commands.CreateTournament;
using SportsGurukul.Application.Features.TournamentManagement.DTOs;
using TournamentTestShared;

namespace Tournament.Application.Tests.CommandHandlers;

public class CreateTournamentCommandHandlerTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ILogger<CreateTournamentCommandHandler>> _loggerMock;
    private readonly CreateTournamentCommandHandler _handler;

    public CreateTournamentCommandHandlerTests()
    {
        _tournamentRepositoryMock = MockRepositoryBuilder.CreateTournamentRepository();
        _unitOfWorkMock = MockUnitOfWorkBuilder.Create();
        _loggerMock = MockLoggerBuilder.Create<CreateTournamentCommandHandler>();
        _handler = new CreateTournamentCommandHandler(
            _tournamentRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_ValidRequest_ReturnsSuccess()
    {
        var command = new CreateTournamentCommand
        {
            TournamentName = "Test Tournament",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            TournamentType = TournamentType.League,
            StartDate = DateTime.UtcNow.AddDays(60),
            EndDate = DateTime.UtcNow.AddDays(67),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(50),
            RegistrationType = RegistrationType.Individual,
            Venue = "Test Venue"
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.TournamentName.Should().Be("Test Tournament");
        result.Value.Status.Should().Be(TournamentStatus.Draft);
        _tournamentRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TournamentEntity>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_EndDateBeforeStartDate_ReturnsFailure()
    {
        var command = new CreateTournamentCommand
        {
            TournamentName = "Test Tournament",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            StartDate = DateTime.UtcNow.AddDays(60),
            EndDate = DateTime.UtcNow.AddDays(30),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = DateTime.UtcNow.AddDays(50)
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("End date must be after start date.");
    }

    [Fact]
    public async Task Handle_RegistrationCloseDateAfterStartDate_ReturnsFailure()
    {
        var startDate = DateTime.UtcNow.AddDays(60);
        var command = new CreateTournamentCommand
        {
            TournamentName = "Test Tournament",
            AcademyId = Guid.NewGuid(),
            SportId = Guid.NewGuid(),
            StartDate = startDate,
            EndDate = startDate.AddDays(7),
            RegistrationOpenDate = DateTime.UtcNow.AddDays(1),
            RegistrationCloseDate = startDate.AddDays(5)
        };

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Registration must close before the tournament starts.");
    }
}
