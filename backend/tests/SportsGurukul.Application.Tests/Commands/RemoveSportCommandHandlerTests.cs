using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RemoveSport;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class RemoveSportCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IRepository<AthleteSport>> _athleteSportRepositoryMock = TestMocks.CreateAthleteSportRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<RemoveSportCommandHandler>> _loggerMock = TestMocks.CreateLogger<RemoveSportCommandHandler>();
    private readonly RemoveSportCommandHandler _handler;

    public RemoveSportCommandHandlerTests()
    {
        _handler = new RemoveSportCommandHandler(
            _athleteRepositoryMock.Object,
            _athleteSportRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_SportNotAssigned_ReturnsFailure()
    {
        var athleteId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        _athleteRepositoryMock.Setup(r => r.GetAthleteSportsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteSport>());

        var result = await _handler.Handle(new RemoveSportCommand
        {
            AthleteId = athleteId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sport is not assigned to the athlete.");
    }

    [Fact]
    public async Task Handle_SportAssigned_RemovesAndReturnsSuccess()
    {
        var athleteId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var athleteSport = TestDataBuilder.CreateAthleteSport(athleteId, sportId);

        _athleteRepositoryMock.Setup(r => r.GetAthleteSportsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteSport> { athleteSport });
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RemoveSportCommand
        {
            AthleteId = athleteId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _athleteSportRepositoryMock.Verify(r => r.Remove(athleteSport), Times.Once);
    }

    [Fact]
    public async Task Handle_SportExistsButSoftDeleted_ReturnsFailure()
    {
        var athleteId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var deletedSport = TestDataBuilder.CreateAthleteSport(athleteId, sportId);
        deletedSport.IsDeleted = true;

        _athleteRepositoryMock.Setup(r => r.GetAthleteSportsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteSport> { deletedSport });

        var result = await _handler.Handle(new RemoveSportCommand
        {
            AthleteId = athleteId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sport is not assigned to the athlete.");
    }

    [Fact]
    public async Task Handle_SportExistsButDifferentId_ReturnsFailure()
    {
        var athleteId = Guid.NewGuid();
        var sportId = Guid.NewGuid();
        var otherSportId = Guid.NewGuid();
        var athleteSport = TestDataBuilder.CreateAthleteSport(athleteId, otherSportId);

        _athleteRepositoryMock.Setup(r => r.GetAthleteSportsAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AthleteSport> { athleteSport });

        var result = await _handler.Handle(new RemoveSportCommand
        {
            AthleteId = athleteId,
            SportId = sportId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("This sport is not assigned to the athlete.");
    }
}
