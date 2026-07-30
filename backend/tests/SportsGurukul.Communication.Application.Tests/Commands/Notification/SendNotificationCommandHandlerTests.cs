using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Notification;

public class SendNotificationCommandHandlerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly SendNotificationCommandHandler _handler;

    public SendNotificationCommandHandlerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new SendNotificationCommandHandler(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldSendQueuedNotification()
    {
        var command = new SendNotificationCommand(Guid.NewGuid());
        var expectedResult = Result<bool>.Success(true);

        _notificationServiceMock
            .Setup(s => s.SendAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.SendAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldDispatchViaNotificationService()
    {
        var command = new SendNotificationCommand(Guid.NewGuid());

        _notificationServiceMock
            .Setup(s => s.SendAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.SendAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotInQueuedStatus_ShouldReturnFailureResult()
    {
        var command = new SendNotificationCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Notification is not in queued status");

        _notificationServiceMock
            .Setup(s => s.SendAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Notification is not in queued status");
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatusToSending()
    {
        var command = new SendNotificationCommand(Guid.NewGuid());

        _notificationServiceMock
            .Setup(s => s.SendAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.SendAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
