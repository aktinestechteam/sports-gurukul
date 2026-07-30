using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public record ScheduleCampaignCommand(Guid CampaignId, DateTime ScheduledAt) : IRequest<Result<bool>>;
