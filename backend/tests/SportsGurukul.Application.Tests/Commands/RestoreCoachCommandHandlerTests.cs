using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.RestoreCoach;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class RestoreCoachCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<RestoreCoachCommandHandler>> _loggerMock = TestMocks.CreateLogger<RestoreCoachCommandHandler>();
    private readonly RestoreCoachCommandHandler _handler;

    public RestoreCoachCommandHandlerTests()
    {
        _handler = new RestoreCoachCommandHandler(
            _coachRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotDeleted_ReturnsFailure()
    {
        var coach = TestDataBuilder.CreateCoach(isDeleted: false);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new RestoreCoachCommand { CoachId = coach.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach is not deleted.");
    }

    [Fact]
    public async Task Handle_DeletedCoach_RestoresAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var deletedCoach = TestDataBuilder.CreateCoach(id: coachId, isDeleted: true);
        deletedCoach.Status = CoachStatus.Inactive;

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(deletedCoach);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new RestoreCoachCommand { CoachId = coachId }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        deletedCoach.IsDeleted.Should().BeFalse();
        deletedCoach.Status.Should().Be(CoachStatus.Active);
        deletedCoach.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _coachRepositoryMock.Verify(r => r.Update(deletedCoach), Times.Once);
    }
}
