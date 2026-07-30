using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Application.Features.NotificationManagement.Queries;
using SportsGurukul.Domain.Entities.Notification;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Communication.Application.Tests.Queries;

public class GetNotificationQueryHandlerTests
{
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly GetNotificationQueryHandler _handler;

    public GetNotificationQueryHandlerTests()
    {
        _notificationServiceMock = new Mock<INotificationService>();
        _handler = new GetNotificationQueryHandler(_notificationServiceMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotification_WhenExists()
    {
        var id = Guid.NewGuid();
        var dto = new NotificationDto(
            id, Guid.NewGuid(), Guid.NewGuid(), "Email", Guid.NewGuid(), "SendGrid",
            NotificationPriority.High, NotificationStatus.Delivered,
            "Subject", "Body", "sender", DateTime.UtcNow, DateTime.UtcNow,
            DateTime.UtcNow, null, null, null, null, null, null, null,
            DateTime.UtcNow, [], []);
        _notificationServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<NotificationDto>.Success(dto));

        var result = await _handler.Handle(new GetNotificationQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(dto);
        result.Value!.Id.Should().Be(id);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenNotFound()
    {
        var id = Guid.NewGuid();
        _notificationServiceMock.Setup(s => s.GetByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<NotificationDto>.Failure($"Notification {id} not found"));

        var result = await _handler.Handle(new GetNotificationQuery(id), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("not found");
    }
}
