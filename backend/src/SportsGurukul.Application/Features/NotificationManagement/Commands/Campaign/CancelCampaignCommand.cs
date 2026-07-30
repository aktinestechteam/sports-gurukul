using MediatR;
using SportsGurukul.Application.Common.Models;

namespace SportsGurukul.Application.Features.NotificationManagement.Commands.Campaign;

public record CancelCampaignCommand(Guid CampaignId) : IRequest<Result<bool>>;
