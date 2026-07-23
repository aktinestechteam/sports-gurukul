using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.AthleteManagement.Commands.DeleteAthlete;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class DeleteAthleteCommandHandlerTests
{
    private readonly Mock<IAthleteRepository> _athleteRepositoryMock = TestMocks.CreateAthleteRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<DeleteAthleteCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeleteAthleteCommandHandler>();
    private readonly DeleteAthleteCommandHandler _handler;

    public DeleteAthleteCommandHandlerTests()
    {
        _handler = new DeleteAthleteCommandHandler(
            _athleteRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_AthleteNotFound_ReturnsFailure()
    {
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Athlete?)null);

        var result = await _handler.Handle(new DeleteAthleteCommand { AthleteId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete not found.");
    }

    [Fact]
    public async Task Handle_AthleteAlreadyDeleted_ReturnsFailure()
    {
        var athlete = TestDataBuilder.CreateAthlete(isDeleted: true);
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);

        var result = await _handler.Handle(new DeleteAthleteCommand { AthleteId = athlete.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Athlete is already deleted.");
    }

    [Fact]
    public async Task Handle_ValidDelete_SoftDeletesAndReturnsSuccess()
    {
        var athlete = TestDataBuilder.CreateAthlete();
        _athleteRepositoryMock.Setup(r => r.GetByIdAsync(athlete.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(athlete);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteAthleteCommand { AthleteId = athlete.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _athleteRepositoryMock.Verify(r => r.Remove(athlete), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
