using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Notification;

public class QueueNotificationCommandHandlerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly QueueNotificationCommandHandler _handler;

    public QueueNotificationCommandHandlerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new QueueNotificationCommandHandler(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldQueueDraftNotification()
    {
        var command = new QueueNotificationCommand(Guid.NewGuid());
        var expectedResult = Result<bool>.Success(true);

        _notificationServiceMock
            .Setup(s => s.QueueAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.QueueAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotificationNotFound_ShouldReturnFailureResult()
    {
        var command = new QueueNotificationCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Notification not found");

        _notificationServiceMock
            .Setup(s => s.QueueAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Notification not found");
    }

    [Fact]
    public async Task Handle_WhenNotificationNotInDraftStatus_ShouldReturnFailureResult()
    {
        var command = new QueueNotificationCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Only draft notifications can be queued");

        _notificationServiceMock
            .Setup(s => s.QueueAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only draft notifications can be queued");
    }

    [Fact]
    public async Task Handle_ShouldUpdateStatusToQueued()
    {
        var command = new QueueNotificationCommand(Guid.NewGuid());

        _notificationServiceMock
            .Setup(s => s.QueueAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.QueueAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }
}
