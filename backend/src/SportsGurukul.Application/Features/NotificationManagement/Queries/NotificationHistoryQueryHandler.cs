using MediatR;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public class NotificationHistoryQueryHandler
    : IRequestHandler<NotificationHistoryQuery, Result<PaginatedResult<NotificationSummaryDto>>>
{
    private readonly INotificationRepository _notificationRepository;

    public NotificationHistoryQueryHandler(INotificationRepository notificationRepository)
    {
        _notificationRepository = notificationRepository;
    }

    public async Task<Result<PaginatedResult<NotificationSummaryDto>>> Handle(
        NotificationHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var query = await _notificationRepository.FindAsync(n => true, cancellationToken);

        var filtered = query.AsEnumerable();

        if (request.UserId.HasValue)
            filtered = filtered.Where(n => n.Recipients.Any(r => r.UserId == request.UserId.Value));

        if (request.FromDate.HasValue)
            filtered = filtered.Where(n => n.CreatedAt >= request.FromDate.Value);

        if (request.ToDate.HasValue)
            filtered = filtered.Where(n => n.CreatedAt <= request.ToDate.Value);

        var list = filtered.OrderByDescending(n => n.CreatedAt).ToList();
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
