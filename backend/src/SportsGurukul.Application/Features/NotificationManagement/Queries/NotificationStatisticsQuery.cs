using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Features.NotificationManagement.Queries;

public record NotificationStatisticsQuery(
    DateTime? FromDate,
    DateTime? ToDate,
    Guid? ChannelId
) : IRequest<Result<NotificationStatisticsDto>>;
