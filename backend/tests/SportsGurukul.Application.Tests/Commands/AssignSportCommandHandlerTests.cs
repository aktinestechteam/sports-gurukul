using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.AssignSport;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class AssignSportCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<ISportRepository> _sportRepositoryMock = TestMocks.CreateSportRepository();
    private readonly Mock<IRepository<AthleteSport>> _athleteSportRepositoryMock = TestMocks.CreateAthleteSportRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<AssignSportCommandHandler>> _loggerMock = TestMocks.CreateLogger<AssignSportCommandHandler>();
    private readonly AssignSportCommandHandler _handler;

    public AssignSportCommandHandlerTests()
    {
        _handler = new AssignSportCommandHandler(
            _athleteRepositoryMock.Object,
            _sportRepositoryMock.Object,
            _athleteSportRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new AssignSportCommand
        {
            AthleteId = Guid.NewGuid(),
            SportId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_SportNotFound_ReturnsFailure()
    {
        var athleteId = Guid.NewGuid();
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateAthlete(id: athleteId));
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sport?)null);

        var result = await _handler.Handle(new AssignSportCommand
        {
            AthleteId = athleteId,
            SportId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sport not found.");
    }

    [Fact]
    public async Task Handle_SportAlreadyAssigned_ReturnsFailure()
    {
        var athleteId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthlete(id: athleteId);
        var sport = TestDataBuilder.CreateSport(id: sportId);
        var existingSports = new List<AthleteSport>
        {
            TestDataBuilder.CreateAthleteSport(athleteId, sportId)
        };

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);
        _athleteRepositoryMock.Setup(r => r.GetAthleteSportsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSports);

        var result = await _handler.Handle(new AssignSportCommand
        {
            AthleteId = athleteId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sport is already assigned to the athlete.");
    }

    [Fact]
    public async Task Handle_NewPrimarySport_UnsetsCurrentPrimary()
    {
        var athleteId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthlete(id: athleteId);
        var sport = TestDataBuilder.CreateSport(id: sportId);
        var currentPrimary = TestDataBuilder.CreateAthleteSport(athleteId);
        currentPrimary.IsPrimarySport = true;

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);
        _athleteRepositoryMock.Setup(r => r.GetAthleteSportsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteSport> { currentPrimary });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AssignSportCommand
        {
            AthleteId = athleteId,
            SportId = sportId,
            IsPrimarySport = true
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        currentPrimary.IsPrimarySport.Should().BeFalse();
        _athleteSportRepositoryMock.Verify(r => r.Update(currentPrimary), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidAssignment_CreatesAndReturnsSuccess()
    {
        var athleteId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var athlete = TestDataBuilder.CreateAthlete(id: athleteId);
        var sport = TestDataBuilder.CreateSport(id: sportId);

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);
        _athleteRepositoryMock.Setup(r => r.GetAthleteSportsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteSport>());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AssignSportCommand
        {
            AthleteId = athleteId,
            SportId = sportId,
            IsPrimarySport = true
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Cricket");
        result.Value.IsPrimarySport.Should().BeTrue();
        _athleteSportRepositoryMock.Verify(r => r.AddAsync(It.IsAny<AthleteSport>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}
