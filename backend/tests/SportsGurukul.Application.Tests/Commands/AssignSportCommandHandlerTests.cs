using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.AssignSport;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class AssignSportCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<ISportRepository> _sportRepositoryMock = TestMocks.CreateSportRepository();
    private readonly Mock<IRepository<CoachSport>> _coachSportRepositoryMock = TestMocks.CreateCoachSportRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<AssignSportCommandHandler>> _loggerMock = TestMocks.CreateLogger<AssignSportCommandHandler>();
    private readonly AssignSportCommandHandler _handler;

    public AssignSportCommandHandlerTests()
    {
        _handler = new AssignSportCommandHandler(
            _coachRepositoryMock.Object,
            _sportRepositoryMock.Object,
            _coachSportRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new AssignSportCommand
        {
            CoachId = Guid.NewGuid(),
            SportId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_SportNotFound_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TestDataBuilder.CreateCoach(id: coachId));
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sport?)null);

        var result = await _handler.Handle(new AssignSportCommand
        {
            CoachId = coachId,
            SportId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Sport not found.");
    }

    [Fact]
    public async Task Handle_DuplicateSport_ReturnsFailure()
    {
        var coachId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId);
        var sport = TestDataBuilder.CreateSport(id: sportId);
        var existingSports = new List<CoachSport>
        {
            TestDataBuilder.CreateCoachSport(coachId, sportId)
        };

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);
        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingSports);

        var result = await _handler.Handle(new AssignSportCommand
        {
            CoachId = coachId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sport is already assigned to the coach.");
    }

    [Fact]
    public async Task Handle_NewSport_AssignsAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId);
        var sport = TestDataBuilder.CreateSport(id: sportId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);
        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachSport>());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AssignSportCommand
        {
            CoachId = coachId,
            SportId = sportId,
            IsPrimarySport = true
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Name.Should().Be("Cricket");
        result.Value.IsPrimarySport.Should().BeTrue();
        _coachSportRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachSport>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_PrimarySport_UnmarksPrevious()
    {
        var coachId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId);
        var sport = TestDataBuilder.CreateSport(id: sportId);
        var currentPrimary = TestDataBuilder.CreateCoachSport(coachId);
        currentPrimary.IsPrimarySport = true;

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _sportRepositoryMock.Setup(r => r.GetByIdAsync(sportId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sport);
        _coachRepositoryMock.Setup(r => r.GetCoachSportsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoachSport> { currentPrimary });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new AssignSportCommand
        {
            CoachId = coachId,
            SportId = sportId,
            IsPrimarySport = true
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        currentPrimary.IsPrimarySport.Should().BeFalse();
        _coachSportRepositoryMock.Verify(r => r.Update(currentPrimary), Times.Once);
    }
}
