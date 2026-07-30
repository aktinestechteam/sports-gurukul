using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class NotificationStatisticsQueryHandler
    : IRequestHandler<NotificationStatisticsQuery, Result<NotificationStatisticsDto>>
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IDeliveryRepository _deliveryRepository;

    public NotificationStatisticsQueryHandler(
        INotificationRepository notificationRepository,
        IDeliveryRepository deliveryRepository)
    {
        _notificationRepository = notificationRepository;
        _deliveryRepository = deliveryRepository;
    }

    public async Task<Result<NotificationStatisticsDto>> Handle(
        NotificationStatisticsQuery request,
        CancellationToken cancellationToken)
    {
        var notifications = await _notificationRepository.FindAsync(n => true, cancellationToken);

        var filtered = notifications.AsEnumerable();

        if (request.FromDate.HasValue)
            filtered = filtered.Where(n => n.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            filtered = filtered.Where(n => n.CreatedAt <= request.ToDate.Value);

        if (request.ChannelId.HasValue)
            filtered = filtered.Where(n => n.ChannelId == request.ChannelId.Value);

        var list = filtered.ToList();

        var total = list.Count;
        var queued = list.Count(n => n.Status == NotificationStatus.Queued);
        var sending = list.Count(n => n.Status == NotificationStatus.Sending);
        var sent = list.Count(n => n.Status == NotificationStatus.Sent);
        var delivered = list.Count(n => n.Status == NotificationStatus.Delivered);
        var failed = list.Count(n => n.Status == NotificationStatus.Failed);
        var cancelled = list.Count(n => n.Status == NotificationStatus.Cancelled);
        var expired = list.Count(n => n.Status == NotificationStatus.Expired);
        var read = list.Count(n => n.Status == NotificationStatus.Read);

        var failureRate = total > 0 ? (double)failed / total * 100 : 0;

        var deliveries = await _deliveryRepository.FindAsync(d => true, cancellationToken);
        var completedDeliveries = deliveries.Where(d => d.DurationMs.HasValue).ToList();
        var avgDeliveryTime = completedDeliveries.Count > 0
            ? completedDeliveries.Average(d => d.DurationMs!.Value)
            : 0;

        return Result<NotificationStatisticsDto>.Success(new NotificationStatisticsDto(
            total, queued, sending, sent, delivered, failed, cancelled, expired,
            read, avgDeliveryTime, failureRate));
    }
}
