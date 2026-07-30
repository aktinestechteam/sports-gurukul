using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public record SearchNotificationsQuery(
    string? SearchTerm,
    NotificationStatus? Status,
    NotificationPriority? Priority,
    Guid? ChannelId,
    Guid? UserId,
    Guid? BatchId,
    Guid? CampaignId,
    DateTime? FromDate,
    DateTime? ToDate,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<PaginatedResult<NotificationSummaryDto>>>;

public record PaginatedResult<T>(
    IReadOnlyList<T> Items,
    int TotalCount,
    int Page,
    int PageSize
);
