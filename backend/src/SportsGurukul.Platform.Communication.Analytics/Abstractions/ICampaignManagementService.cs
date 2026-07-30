using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface ICampaignManagementService
{
    Task<CampaignDetailDto> CreateAsync(CreateCampaignFullRequest request, string? createdBy, CancellationToken ct = default);
    Task<CampaignDetailDto> UpdateAsync(Guid id, UpdateCampaignRequest request, CancellationToken ct = default);
    Task<CampaignDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CampaignSearchResult> SearchAsync(CampaignSearchCriteria criteria, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<CampaignDetailDto> ActivateAsync(Guid id, CancellationToken ct = default);
    Task<PauseCampaignResult> PauseAsync(Guid id, CancellationToken ct = default);
    Task<ResumeCampaignResult> ResumeAsync(Guid id, CancellationToken ct = default);
    Task<CampaignDetailDto> CancelAsync(Guid id, CancellationToken ct = default);
    Task<CampaignDetailDto> CloneAsync(Guid id, CampaignCloneRequest request, CancellationToken ct = default);
    Task<CampaignTriggerResult> TriggerNowAsync(Guid id, CancellationToken ct = default);
    Task<CampaignTriggerResult> TriggerScheduledAsync(Guid id, CancellationToken ct = default);
    Task<CampaignBulkCreateResult> BulkCreateAsync(CampaignBulkCreateRequest request, string? createdBy, CancellationToken ct = default);
    Task<int> GetCountByStatusAsync(CampaignStatus status, CancellationToken ct = default);
    Task<List<CampaignDetailDto>> GetDueCampaignsAsync(CancellationToken ct = default);
    Task<CampaignDetailDto> UpdateScheduleAsync(Guid id, ScheduleDefinitionDto schedule, CancellationToken ct = default);
    Task<CampaignDetailDto> UpdateAudienceAsync(Guid id, AudienceDefinitionDto audience, CancellationToken ct = default);
}
