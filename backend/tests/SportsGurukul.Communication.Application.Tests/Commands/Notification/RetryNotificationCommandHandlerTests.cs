using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Notification;

public class RetryNotificationCommandHandlerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly RetryNotificationCommandHandler _handler;

    public RetryNotificationCommandHandlerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new RetryNotificationCommandHandler(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldRetryFailedNotification()
    {
        var command = new RetryNotificationCommand(Guid.NewGuid());
        var expectedResult = Result<bool>.Success(true);

        _notificationServiceMock
            .Setup(s => s.RetryAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.RetryAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldResetAttemptCounter()
    {
        var command = new RetryNotificationCommand(Guid.NewGuid());

        _notificationServiceMock
            .Setup(s => s.RetryAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.RetryAsync(command.Id, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotificationNotFailed_ShouldReturnFailureResult()
    {
        var command = new RetryNotificationCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Only failed notifications can be retried");

        _notificationServiceMock
            .Setup(s => s.RetryAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only failed notifications can be retried");
    }

    [Fact]
    public async Task Handle_WhenMaxRetriesExceeded_ShouldReturnFailureResult()
    {
        var command = new RetryNotificationCommand(Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Maximum retry attempts exceeded");

        _notificationServiceMock
            .Setup(s => s.RetryAsync(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Maximum retry attempts exceeded");
    }
}
