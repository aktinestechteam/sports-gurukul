using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Notification;

public class CancelNotificationCommandHandlerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly CancelNotificationCommandHandler _handler;

    public CancelNotificationCommandHandlerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new CancelNotificationCommandHandler(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCancelNotification()
    {
        var command = new CancelNotificationCommand(Guid.NewGuid());
        var expectedResult = Result<bool>.Success(true);

        _notificationServiceMock
            .Setup(s => s.CancelAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.CancelAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatusToCancelled()
    {
        var command = new CancelNotificationCommand(Guid.NewGuid());

        _notificationServiceMock
            .Setup(s => s.CancelAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.CancelAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenAlreadyCompleted_ShouldReturnFailureResult()
    {
        var command = new CancelNotificationCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Cannot cancel a completed notification");

        _notificationServiceMock
            .Setup(s => s.CancelAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Cannot cancel a completed notification");
    }

    [Fact]
    public async Task Handle_WhenAlreadyCancelled_ShouldReturnFailureResult()
    {
        var command = new CancelNotificationCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Notification is already cancelled");

        _notificationServiceMock
            .Setup(s => s.CancelAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Notification is already cancelled");
    }
}
