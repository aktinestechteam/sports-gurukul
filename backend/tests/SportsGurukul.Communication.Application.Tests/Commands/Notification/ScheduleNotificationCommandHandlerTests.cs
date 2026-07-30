using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Notification;

public class ScheduleNotificationCommandHandlerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly ScheduleNotificationCommandHandler _handler;

    public ScheduleNotificationCommandHandlerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new ScheduleNotificationCommandHandler(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldScheduleNotification()
    {
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), new DateTime(2026, 8, 15, 9, 0, 0, DateTimeKind.Utc));
        var expectedResult = Result<bool>.Success(true);

        _notificationServiceMock
            .Setup(s => s.ScheduleAsync(command.Id, command.ScheduledAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.ScheduleAsync(command.Id, command.ScheduledAt, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldSetScheduledTime()
    {
        var scheduledAt = new DateTime(2026, 9, 1, 14, 30, 0, DateTimeKind.Utc);
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), scheduledAt);

        _notificationServiceMock
            .Setup(s => s.ScheduleAsync(command.Id, command.ScheduledAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.ScheduleAsync(
            command.Id,
            It.Is<DateTime>(dt => dt == scheduledAt),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotificationNotFound_ShouldReturnFailureResult()
    {
        var command = new ScheduleNotificationCommand(Guid.NewGuid(), DateTime.UtcNow);
        var failureResult = Result<bool>.Failure("Notification not found");

        _notificationServiceMock
            .Setup(s => s.ScheduleAsync(command.Id, command.ScheduledAt, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Notification not found");
    }
}
