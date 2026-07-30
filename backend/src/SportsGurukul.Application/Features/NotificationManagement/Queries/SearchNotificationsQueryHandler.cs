using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class SearchNotificationsQueryHandler
    : IRequestHandler<SearchNotificationsQuery, Result<PaginatedResult<NotificationSummaryDto>>>
{
    private readonly INotificationRepository _notificationRepository;

    public SearchNotificationsQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<PaginatedResult<NotificationSummaryDto>>> Handle(
        SearchNotificationsQuery request,
        CancellationToken cancellationToken)
    {
        var query = await _notificationRepository.FindAsync(n => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (request.Status.HasValue)
            filtered = filtered.Where(n => n.Status == request.Status.Value);

        if (request.Priority.HasValue)
            filtered = filtered.Where(n => n.Priority == request.Priority.Value);

        if (request.ChannelId.HasValue)
            filtered = filtered.Where(n => n.ChannelId == request.ChannelId.Value);

        if (request.BatchId.HasValue)
            filtered = filtered.Where(n => n.BatchId == request.BatchId.Value);

        if (request.CampaignId.HasValue)
            filtered = filtered.Where(n => n.CampaignId == request.CampaignId.Value);

        if (request.FromDate.HasValue)
            filtered = filtered.Where(n => n.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            filtered = filtered.Where(n => n.CreatedAt <= request.ToDate.Value);

        var list = filtered.ToList();
        var total = list.Count;
        var paged = list
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(n => new NotificationSummaryDto(
                n.Id, n.Priority, n.Status, n.Subject,
                string.Empty, n.Recipients.Count,
                n.SentAt, n.CreatedAt))
            .ToList();

        return Result<PaginatedResult<NotificationSummaryDto>>.Success(
            new PaginatedResult<NotificationSummaryDto>(paged, total, request.Page, request.PageSize));
    }
}
