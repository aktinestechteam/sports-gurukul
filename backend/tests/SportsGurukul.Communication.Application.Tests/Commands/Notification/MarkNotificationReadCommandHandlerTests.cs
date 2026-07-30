using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.Commands.Notification;

namespace SportsGurukul.Communication.Application.Tests.Commands.Notification;

public class MarkNotificationReadCommandHandlerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly MarkNotificationReadCommandHandler _handler;

    public MarkNotificationReadCommandHandlerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new MarkNotificationReadCommandHandler(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldMarkNotificationAsRead()
    {
        var command = new MarkNotificationReadCommand(Guid.NewGuid(), Guid.NewGuid());
        var expectedResult = Result<bool>.Success(true);

        _notificationServiceMock
            .Setup(s => s.MarkReadAsync(command.Id, command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.MarkReadAsync(command.Id, command.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldUpdateReadTimestamp()
    {
        var command = new MarkNotificationReadCommand(Guid.NewGuid(), Guid.NewGuid());

        _notificationServiceMock
            .Setup(s => s.MarkReadAsync(command.Id, command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<bool>.Success(true));

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.MarkReadAsync(command.Id, command.UserId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_WhenNotificationNotDelivered_ShouldReturnFailureResult()
    {
        var command = new MarkNotificationReadCommand(Guid.NewGuid(), Guid.NewGuid());
        var failureResult = Result<bool>.Failure("Only delivered notifications can be marked as read");

        _notificationServiceMock
            .Setup(s => s.MarkReadAsync(command.Id, command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(failureResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Be("Only delivered notifications can be marked as read");
    }

    [Fact]
    public async Task Handle_WithNullUserId_ShouldMarkNotificationAsRead()
    {
        var command = new MarkNotificationReadCommand(Guid.NewGuid(), null);
        var expectedResult = Result<bool>.Success(true);

        _notificationServiceMock
            .Setup(s => s.MarkReadAsync(command.Id, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        _notificationServiceMock.Verify(s => s.MarkReadAsync(command.Id, null, It.IsAny<CancellationToken>()), Times.Once);
    }
}
