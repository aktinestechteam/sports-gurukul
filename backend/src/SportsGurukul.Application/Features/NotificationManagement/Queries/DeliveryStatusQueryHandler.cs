using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class DeliveryStatusQueryHandler
    : IRequestHandler<DeliveryStatusQuery, Result<List<DeliveryDto>>>
{
    private readonly IDeliveryTrackingService _deliveryTrackingService;

    public DeliveryStatusQueryHandler(IDeliveryTrackingService deliveryTrackingService)
    {
        _deliveryTrackingService = deliveryTrackingService;
    }

    public async Task<Result<List<DeliveryDto>>> Handle(
        DeliveryStatusQuery request,
        CancellationToken cancellationToken)
    {
        return await _deliveryTrackingService.GetByNotificationIdAsync(request.NotificationId, cancellationToken);
    }
}
