using FluentAssertions;
using MediatR;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.DeleteCoach;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class DeleteCoachCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<DeleteCoachCommandHandler>> _loggerMock = TestMocks.CreateLogger<DeleteCoachCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly DeleteCoachCommandHandler _handler;

    public DeleteCoachCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new DeleteCoachCommandHandler(
            _coachRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new DeleteCoachCommand { CoachId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_CoachAlreadyDeleted_ReturnsFailure()
    {
        var coach = TestDataBuilder.CreateCoach(isDeleted: true, userId: _testUserId);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);

        var result = await _handler.Handle(new DeleteCoachCommand { CoachId = coach.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach is already deleted.");
    }

    [Fact]
    public async Task Handle_ValidCoach_SoftDeletesAndReturnsSuccess()
    {
        var coach = TestDataBuilder.CreateCoach(userId: _testUserId);
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coach.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new DeleteCoachCommand { CoachId = coach.Id }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(Unit.Value);
        _coachRepositoryMock.Verify(r => r.Remove(coach), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
