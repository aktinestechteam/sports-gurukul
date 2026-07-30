using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

namespace SportsGurukul.Application.Common.Interfaces.Notification.Services;

public interface ICampaignService
{
    Task<Result<CampaignDto>> CreateAsync(CreateCampaignRequest request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ScheduleAsync(Guid campaignId, DateTime scheduledAt, CancellationToken cancellationToken = default);
    Task<Result<bool>> CancelAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<Result<bool>> PauseAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<Result<bool>> ResumeAsync(Guid campaignId, CancellationToken cancellationToken = default);
    Task<Result<CampaignDto>> GetByIdAsync(Guid campaignId, CancellationToken cancellationToken = default);
}
