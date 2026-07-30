using MediatR;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public record CreateCampaignCommand(
    string Name,
    string? Description,
    Guid? TemplateId,
    NotificationChannelType ChannelType,
    DateTime? ScheduledAt,
    string? TargetCriteria,
    string? Metadata
) : IRequest<Result<CampaignDto>>;
