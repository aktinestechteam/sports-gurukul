using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface ITemplateManagementService
{
    Task<TemplateDetailDto> CreateAsync(CreateTemplateFullRequest request, string? createdBy, CancellationToken ct = default);
    Task<TemplateDetailDto> UpdateAsync(Guid id, UpdateTemplateFullRequest request, CancellationToken ct = default);
    Task<TemplateDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<TemplateSearchResult> SearchAsync(TemplateSearchCriteria criteria, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    Task<TemplateDetailDto> PublishAsync(Guid id, string? publishedBy, CancellationToken ct = default);
    Task<TemplateDetailDto> ArchiveAsync(Guid id, CancellationToken ct = default);
    Task<TemplateDetailDto> DraftAsync(Guid id, CancellationToken ct = default);
    Task<TemplateDetailDto> CloneAsync(Guid id, CloneTemplateRequest request, CancellationToken ct = default);
    Task<TemplateDetailDto> RollbackAsync(Guid id, RollbackTemplateRequest request, CancellationToken ct = default);
    Task<TemplateVersionDetailDto> CreateVersionAsync(Guid templateId, CreateTemplateVersionRequest request, CancellationToken ct = default);
    Task<List<TemplateVersionDetailDto>> GetVersionsAsync(Guid templateId, CancellationToken ct = default);
    Task<TemplateVersionCompareDto> CompareVersionsAsync(Guid templateId, int fromVersion, int toVersion, CancellationToken ct = default);
    Task<TemplateRenderPreviewResult> PreviewAsync(TemplateRenderPreviewRequest request, CancellationToken ct = default);
    Task<TemplateRenderPreviewResult> PreviewTemplateAsync(Guid templateId, Dictionary<string, object?> testData, string? locale, CancellationToken ct = default);
    Task<TemplateLocalizationDto> AddLocalizationAsync(Guid templateId, CreateLocalizationRequest request, CancellationToken ct = default);
    Task<List<TemplateLocalizationDto>> GetLocalizationsAsync(Guid templateId, CancellationToken ct = default);
    Task<TemplateAttachmentMetaDto> AddAttachmentMetaAsync(Guid templateId, CreateAttachmentMetaRequest request, CancellationToken ct = default);
    Task<bool> RemoveAttachmentMetaAsync(Guid templateId, Guid attachmentId, CancellationToken ct = default);
    Task<TemplateDetailDto> UpdateCategoryAsync(Guid id, TemplateCategory category, CancellationToken ct = default);
    Task<List<TemplateCategory>> GetCategoriesAsync(CancellationToken ct = default);
    Task<int> GetTemplateCountByStatusAsync(TemplateStatus status, CancellationToken ct = default);
    Task<Dictionary<TemplateCategory, int>> GetTemplateCountByCategoryAsync(CancellationToken ct = default);
}
