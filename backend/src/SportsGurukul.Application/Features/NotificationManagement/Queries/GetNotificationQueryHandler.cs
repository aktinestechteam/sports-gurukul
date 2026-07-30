using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class GetNotificationQueryHandler
    : IRequestHandler<GetNotificationQuery, Result<NotificationDto>>
{
    private readonly INotificationService _notificationService;

    public GetNotificationQueryHandler(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    public async Task<Result<NotificationDto>> Handle(
        GetNotificationQuery request,
        CancellationToken cancellationToken)
    {
        return await _notificationService.GetByIdAsync(request.Id, cancellationToken);
    }
}
