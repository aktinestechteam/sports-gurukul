using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeactivateCoach;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;
using SportsGurukul.Domain.Enums;

namespace SportsGurukul.Application.Tests.Commands;

public class DeactivateCoachCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<DeactivateCoachCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeactivateCoachCommandHandler>();
    private readonly DeactivateCoachCommandHandler _handler;

    public DeactivateCoachCommandHandlerTests()
    {
        _handler = new DeactivateCoachCommandHandler(
            _coachRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new DeactivateCoachCommand
        {
            CoachId = Guid.NewGuid()
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_ValidDeactivation_SetsInactiveAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoachWithDetails(id: coachId);
        coach.Status = CoachStatus.Active;

        _coachRepositoryMock.Setup(r => r.GetByIdWithDetailsAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeactivateCoachCommand
        {
            CoachId = coachId
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Status.Should().Be("Inactive");
        coach.Status.Should().Be(CoachStatus.Inactive);
        _coachRepositoryMock.Verify(r => r.Update(coach), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
