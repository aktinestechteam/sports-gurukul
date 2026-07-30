using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class TemplateVersionService : ITemplateVersionService
{
    private readonly ITemplateManagementService _templateManagement;
    private readonly ILogger<TemplateVersionService> _logger;

    public TemplateVersionService(
        ITemplateManagementService templateManagement,
        ILogger<TemplateVersionService> logger)
    {
        _templateManagement = templateManagement;
        _logger = logger;
    }

    public async Task<TemplateVersionDetailDto> CreateVersionAsync(Guid templateId, CreateTemplateVersionRequest request, CancellationToken ct = default)
    {
        var template = await _templateManagement.GetByIdAsync(templateId, ct);
        var version = new TemplateVersionDetailDto(
            Guid.NewGuid(),
            template.CurrentVersion + 1,
            request.SubjectTemplate ?? template.SubjectTemplate,
            request.BodyTemplate ?? template.BodyTemplate,
            request.ChangeNotes,
            TemplateStatus.Draft,
            null,
            DateTime.UtcNow,
            null
        );
        _logger.LogInformation("Created version {Version} for template {TemplateId}", version.VersionNumber, templateId);
        return version;
    }

    public async Task<TemplateVersionDetailDto> GetVersionAsync(Guid templateId, int versionNumber, CancellationToken ct = default)
    {
        var versions = await _templateManagement.GetVersionsAsync(templateId, ct);
        return versions.FirstOrDefault(v => v.VersionNumber == versionNumber)
            ?? throw new KeyNotFoundException($"Version {versionNumber} not found for template {templateId}");
    }

    public async Task<List<TemplateVersionDetailDto>> GetAllVersionsAsync(Guid templateId, CancellationToken ct = default)
    {
        return await _templateManagement.GetVersionsAsync(templateId, ct);
    }

    public async Task<TemplateVersionCompareDto> CompareVersionsAsync(Guid templateId, int fromVersion, int toVersion, CancellationToken ct = default)
    {
        return await _templateManagement.CompareVersionsAsync(templateId, fromVersion, toVersion, ct);
    }

    public async Task<TemplateVersionDetailDto> PublishVersionAsync(Guid templateId, int versionNumber, string? publishedBy, CancellationToken ct = default)
    {
        var version = await GetVersionAsync(templateId, versionNumber, ct);
        var published = version with
        {
            Status = TemplateStatus.Published,
            PublishedBy = publishedBy,
            PublishedAt = DateTime.UtcNow
        };
        _logger.LogInformation("Published version {Version} of template {TemplateId}", versionNumber, templateId);
        return published;
    }

    public async Task RollbackToVersionAsync(Guid templateId, int versionNumber, string? changeNotes, CancellationToken ct = default)
    {
        await _templateManagement.RollbackAsync(templateId, new RollbackTemplateRequest(versionNumber, changeNotes), ct);
        _logger.LogInformation("Rolled back template {TemplateId} to version {Version}", templateId, versionNumber);
    }

    public async Task<int> GetVersionCountAsync(Guid templateId, CancellationToken ct = default)
    {
        var versions = await _templateManagement.GetVersionsAsync(templateId, ct);
        return versions.Count;
    }

    public async Task<TemplateVersionDetailDto> GetLatestVersionAsync(Guid templateId, CancellationToken ct = default)
    {
        var versions = await _templateManagement.GetVersionsAsync(templateId, ct);
        return versions.OrderByDescending(v => v.VersionNumber).First();
    }
}
