using FluentAssertions;
using Moq;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Features.CoachManagement.Commands.UpdateAvailability;
using SportsGurukul.Application.Tests.Common;
using SportsGurukul.Domain.Entities;

namespace SportsGurukul.Application.Tests.Commands;

public class UpdateAvailabilityCommandHandlerTests
{
    private readonly Mock<ICoachRepository> _coachRepositoryMock = TestMocks.CreateCoachRepository();
    private readonly Mock<ICoachAvailabilityRepository> _availabilityRepositoryMock = TestMocks.CreateCoachAvailabilityRepository();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = TestMocks.CreateUnitOfWork();
    private readonly Mock<ILogger<UpdateAvailabilityCommandHandler>> _loggerMock = TestMocks.CreateLogger<UpdateAvailabilityCommandHandler>();
    private readonly Mock<ICurrentUser> _currentUserMock = new();
    private readonly Guid _testUserId = Guid.NewGuid();
    private readonly UpdateAvailabilityCommandHandler _handler;

    public UpdateAvailabilityCommandHandlerTests()
    {
        _currentUserMock.Setup(u => u.Roles).Returns(new List<string> { "Coach" });
        _currentUserMock.Setup(u => u.UserId).Returns(_testUserId);
        _handler = new UpdateAvailabilityCommandHandler(
            _coachRepositoryMock.Object,
            _availabilityRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _loggerMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task Handle_CoachNotFound_ReturnsFailure()
    {
        _coachRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Coach?)null);

        var result = await _handler.Handle(new UpdateAvailabilityCommand { CoachId = Guid.NewGuid() }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Coach not found.");
    }

    [Fact]
    public async Task Handle_NoExistingAvailability_CreatesAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _availabilityRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CoachAvailability?)null);
        _availabilityRepositoryMock.Setup(r => r.AddAsync(It.IsAny<CoachAvailability>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CoachAvailability());
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateAvailabilityCommand
        {
            CoachId = coachId,
            WeeklySchedule = "{\"mon\":\"9-5\"}",
            TimeSlots = "[\"09:00\",\"10:00\"]",
            OnlineAvailable = true,
            OfflineAvailable = false,
            TravelDistance = 50
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.WeeklySchedule.Should().Be("{\"mon\":\"9-5\"}");
        result.Value.TimeSlots.Should().Be("[\"09:00\",\"10:00\"]");
        result.Value.OnlineAvailable.Should().BeTrue();
        result.Value.OfflineAvailable.Should().Be(false);
        result.Value.TravelDistance.Should().Be(50);
        _availabilityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachAvailability>(), It.IsAny<CancellationToken>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ExistingAvailability_UpdatesAndReturnsSuccess()
    {
        var coachId = Guid.NewGuid();
        var coach = TestDataBuilder.CreateCoach(id: coachId, userId: _testUserId);
        var existingAvailability = TestDataBuilder.CreateCoachAvailability(coachId);

        _coachRepositoryMock.Setup(r => r.GetByIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coach);
        _availabilityRepositoryMock.Setup(r => r.GetByCoachIdAsync(coachId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingAvailability);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var result = await _handler.Handle(new UpdateAvailabilityCommand
        {
            CoachId = coachId,
            WeeklySchedule = "{\"tue\":\"10-6\"}",
            OnlineAvailable = false,
            TravelDistance = 100
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        existingAvailability.WeeklySchedule.Should().Be("{\"tue\":\"10-6\"}");
        existingAvailability.OnlineAvailable.Should().BeFalse();
        existingAvailability.TravelDistance.Should().Be(100);
        existingAvailability.UpdatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        _availabilityRepositoryMock.Verify(r => r.Update(existingAvailability), Times.Once);
        _availabilityRepositoryMock.Verify(r => r.AddAsync(It.IsAny<CoachAvailability>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
