using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Abstractions;

public interface ITemplateVersionService
{
    Task<TemplateVersionDetailDto> CreateVersionAsync(Guid templateId, CreateTemplateVersionRequest request, CancellationToken ct = default);
    Task<TemplateVersionDetailDto> GetVersionAsync(Guid templateId, int versionNumber, CancellationToken ct = default);
    Task<List<TemplateVersionDetailDto>> GetAllVersionsAsync(Guid templateId, CancellationToken ct = default);
    Task<TemplateVersionCompareDto> CompareVersionsAsync(Guid templateId, int fromVersion, int toVersion, CancellationToken ct = default);
    Task<TemplateVersionDetailDto> PublishVersionAsync(Guid templateId, int versionNumber, string? publishedBy, CancellationToken ct = default);
    Task RollbackToVersionAsync(Guid templateId, int versionNumber, string? changeNotes, CancellationToken ct = default);
    Task<int> GetVersionCountAsync(Guid templateId, CancellationToken ct = default);
    Task<TemplateVersionDetailDto> GetLatestVersionAsync(Guid templateId, CancellationToken ct = default);
}
