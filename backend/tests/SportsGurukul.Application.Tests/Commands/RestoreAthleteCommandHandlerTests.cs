using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.RestoreAthlete;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class RestoreAthleteCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<RestoreAthleteCommandHandler>> _loggerMock = TestMocks.CreateLogger<RestoreAthleteCommandHandler>();
    private readonly RestoreAthleteCommandHandler _handler;

    public RestoreAthleteCommandHandlerTests()
    {
        _handler = new RestoreAthleteCommandHandler(
            _athleteRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotDeleted_ReturnsFailure()
    {
        var athlete = TestDataBuilder.CreateAthlete(isDeleted: false);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(new RestoreAthleteCommand { AthleteId = athlete.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete is not deleted.");
    }

    [Fact]
    public async Task Handle_DeletedAthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _athleteRepositoryMock.Setup(r => r.GetDeletedByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new RestoreAthleteCommand { AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Deleted athlete not found.");
    }

    [Fact]
    public async Task Handle_DeletedAthleteExists_RestoresAndReturnsSuccess()
    {
        var athleteId = Guid.NewGuid();
        var deletedAthlete = TestDataBuilder.CreateAthlete(id: athleteId, isDeleted: true);

        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);
        _athleteRepositoryMock.Setup(r => r.GetDeletedByIdAsync(athleteId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedAthlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RestoreAthleteCommand { AthleteId = athleteId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        deletedAthlete.IsDeleted.Should().BeFalse();
        deletedAthlete.Status.Should().Be(AthleteStatus.Active);
        deletedAthlete.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _athleteRepositoryMock.Verify(r => r.Update(deletedAthlete), Times.Once);
    }
}
