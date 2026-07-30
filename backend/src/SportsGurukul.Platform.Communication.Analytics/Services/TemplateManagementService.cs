using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Enums.Notification;
using SportsGurukul.Platform.Communication.Analytics.Abstractions;
using SportsGurukul.Platform.Communication.Analytics.Configuration;
using SportsGurukul.Platform.Communication.Analytics.DTOs;

namespace SportsGurukul.Platform.Communication.Analytics.Services;

public class TemplateManagementService : ITemplateManagementService
{
    private readonly ILogger<TemplateManagementService> _logger;
    private readonly ICacheService _cache;
    private readonly ITemplateService _templateService;

    private readonly ConcurrentDictionary<Guid, TemplateDetailDto> _templates = new();
    private readonly ConcurrentDictionary<Guid, List<TemplateVersionDetailDto>> _versions = new();
    private readonly ConcurrentDictionary<Guid, List<TemplateLocalizationDto>> _localizations = new();
    private readonly ConcurrentDictionary<Guid, List<TemplateAttachmentMetaDto>> _attachments = new();
    private readonly ConcurrentDictionary<string, TemplatePartialDto> _partials = new();
    private int _nextVersionNumber = 1;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public TemplateManagementService(
        ILogger<TemplateManagementService> logger,
        ICacheService cache,
        ITemplateService templateService)
    {
        _logger = logger;
        _cache = cache;
        _templateService = templateService;
        SeedSamplePartials();
    }

    public async Task<TemplateDetailDto> CreateAsync(CreateTemplateFullRequest request, string? createdBy, CancellationToken ct = default)
    {
        var id = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var versionNumber = Interlocked.Increment(ref _nextVersionNumber);

        var resolvedVariables = (request.Variables ?? [])
            .Select(v => new TemplateVariableDetailDto(
                Guid.NewGuid(), v.Name, v.Description, v.IsRequired,
                v.DefaultValue, string.Empty, null, null, null, null))
            .ToList();

        var resolvedLocalizations = (request.Localizations ?? [])
            .Select(l => new TemplateLocalizationDto(
                Guid.NewGuid(), l.Locale, l.SubjectTemplate, l.BodyTemplate, true, now))
            .ToList();

        var resolvedAttachments = (request.Attachments ?? [])
            .Select(a => new TemplateAttachmentMetaDto(
                Guid.NewGuid(), a.FileName, a.ContentType, a.SizeBytes, a.IsRequired, a.Description))
            .ToList();

        var version = new TemplateVersionDetailDto(
            Guid.NewGuid(), versionNumber, request.SubjectTemplate, request.BodyTemplate,
            "Initial version", TemplateStatus.Draft, null, now, null);

        var dto = new TemplateDetailDto(
            id, request.Name, request.Description, request.ChannelType, request.Category,
            TemplateStatus.Draft, request.SubjectTemplate, request.BodyTemplate, true,
            versionNumber, now, null, null, createdBy, null,
            new List<TemplateVersionDetailDto> { version },
            resolvedVariables, resolvedLocalizations,
            (request.PartialNames ?? []).Select(pn => ResolvePartial(pn)).Where(p => p != null).Select(p => p!).ToList(),
            resolvedAttachments, request.Metadata);

        _templates[id] = dto;
        _versions[id] = new List<TemplateVersionDetailDto> { version };

        if (request.Localizations?.Count > 0)
            _localizations[id] = resolvedLocalizations;

        if (request.Attachments?.Count > 0)
            _attachments[id] = resolvedAttachments;

        try
        {
            var createRequest = new CreateTemplateRequest(
                request.Name, request.Description, request.ChannelType,
                request.SubjectTemplate, request.BodyTemplate,
                request.Variables?.Select(v => new CreateTemplateVariableRequest(
                    v.Name, v.Description, v.IsRequired, v.DefaultValue, string.Empty)).ToList());

            await _templateService.CreateAsync(createRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist template {TemplateId} via ITemplateService, using in-memory store", id);
        }

        await _cache.SetAsync(CacheKeys.TemplateKey(id), dto, CacheDuration);

        _logger.LogInformation("Created template {TemplateId} '{Name}' version {Version}", id, request.Name, versionNumber);
        return dto;
    }

    public async Task<TemplateDetailDto> UpdateAsync(Guid id, UpdateTemplateFullRequest request, CancellationToken ct = default)
    {
        if (!_templates.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Template {id} not found");

        var now = DateTime.UtcNow;
        var versionNumber = Interlocked.Increment(ref _nextVersionNumber);
        var changedFields = new List<string>();

        var name = request.Name ?? existing.Name;
        if (request.Name != null) changedFields.Add("Name");

        var description = request.Description ?? existing.Description;
        if (request.Description != null) changedFields.Add("Description");

        var category = request.Category ?? existing.Category;
        if (request.Category != null) changedFields.Add("Category");

        var subjectTemplate = request.SubjectTemplate ?? existing.SubjectTemplate;
        if (request.SubjectTemplate != null) changedFields.Add("SubjectTemplate");

        var bodyTemplate = request.BodyTemplate ?? existing.BodyTemplate;
        if (request.BodyTemplate != null) changedFields.Add("BodyTemplate");

        var version = new TemplateVersionDetailDto(
            Guid.NewGuid(), versionNumber, subjectTemplate, bodyTemplate,
            $"Updated: {string.Join(", ", changedFields)}", existing.Status, null, now, null);

        _versions.AddOrUpdate(id,
            _ => new List<TemplateVersionDetailDto> { version },
            (_, list) => { list.Add(version); return list; });

        if (request.Variables != null)
        {
            existing = existing with
            {
                Variables = request.Variables.Select(v => new TemplateVariableDetailDto(
                    Guid.NewGuid(), v.Name, v.Description, v.IsRequired,
                    v.DefaultValue, string.Empty, null, null, null, null)).ToList()
            };
        }

        if (request.Localizations != null)
        {
            var locs = request.Localizations.Select(l => new TemplateLocalizationDto(
                Guid.NewGuid(), l.Locale, l.SubjectTemplate, l.BodyTemplate, true, now)).ToList();
            _localizations[id] = locs;
        }

        if (request.Attachments != null)
        {
            var atts = request.Attachments.Select(a => new TemplateAttachmentMetaDto(
                Guid.NewGuid(), a.FileName, a.ContentType, a.SizeBytes, a.IsRequired, a.Description)).ToList();
            _attachments[id] = atts;
        }

        List<TemplatePartialDto> partials;
        if (request.PartialNames != null)
        {
            partials = request.PartialNames.Select(pn => ResolvePartial(pn)).Where(p => p != null).Select(p => p!).ToList();
        }
        else
        {
            partials = existing.Partials;
        }

        var metadata = request.Metadata ?? existing.Metadata;

        var updated = existing with
        {
            Name = name,
            Description = description,
            Category = category,
            SubjectTemplate = subjectTemplate,
            BodyTemplate = bodyTemplate,
            CurrentVersion = versionNumber,
            Versions = existing.Versions.Append(version).ToList(),
            Partials = partials,
            Metadata = metadata
        };

        _templates[id] = updated;

        try
        {
            var updateRequest = new UpdateTemplateRequest(
                id, request.Name, request.Description, request.SubjectTemplate,
                request.BodyTemplate, request.Variables?.Select(v => new CreateTemplateVariableRequest(
                    v.Name, v.Description, v.IsRequired, v.DefaultValue, string.Empty)).ToList());

            await _templateService.UpdateAsync(updateRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update template {TemplateId} via ITemplateService", id);
        }

        await _cache.SetAsync(CacheKeys.TemplateKey(id), updated, CacheDuration);

        _logger.LogInformation("Updated template {TemplateId} to version {Version}", id, versionNumber);
        return updated;
    }

    public async Task<TemplateDetailDto> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var cached = await _cache.GetAsync<TemplateDetailDto>(CacheKeys.TemplateKey(id));
        if (cached != null)
            return cached;

        if (_templates.TryGetValue(id, out var template))
        {
            await _cache.SetAsync(CacheKeys.TemplateKey(id), template, CacheDuration);
            return template;
        }

        try
        {
            var result = await _templateService.GetByIdAsync(id, ct);
            if (result.IsSuccess && result.Value != null)
            {
                var t = result.Value;
                var dto = MapFromTemplateDto(t);
                _templates[id] = dto;
                await _cache.SetAsync(CacheKeys.TemplateKey(id), dto, CacheDuration);
                return dto;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load template {TemplateId} from ITemplateService", id);
        }

        throw new KeyNotFoundException($"Template {id} not found");
    }

    public Task<TemplateSearchResult> SearchAsync(TemplateSearchCriteria criteria, CancellationToken ct = default)
    {
        var query = _templates.Values.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(criteria.Query))
        {
            var q = criteria.Query.ToLowerInvariant();
            query = query.Where(t =>
                t.Name.ToLowerInvariant().Contains(q) ||
                (t.Description?.ToLowerInvariant().Contains(q) ?? false));
        }

        if (criteria.Category.HasValue)
            query = query.Where(t => t.Category == criteria.Category.Value);

        if (criteria.Status.HasValue)
            query = query.Where(t => t.Status == criteria.Status.Value);

        if (criteria.ChannelType.HasValue)
            query = query.Where(t => t.ChannelType == criteria.ChannelType.Value);

        if (criteria.CreatedAfter.HasValue)
            query = query.Where(t => t.CreatedAt >= criteria.CreatedAfter.Value);

        if (criteria.CreatedBefore.HasValue)
            query = query.Where(t => t.CreatedAt <= criteria.CreatedBefore.Value);

        if (!string.IsNullOrWhiteSpace(criteria.CreatedBy))
            query = query.Where(t => t.CreatedBy == criteria.CreatedBy);

        if (criteria.HasLocalizations.HasValue)
            query = query.Where(t => (t.Localizations.Count > 0) == criteria.HasLocalizations.Value);

        var totalCount = query.Count();
        var pageNumber = Math.Max(1, criteria.PageNumber);
        var pageSize = Math.Clamp(criteria.PageSize, 1, 100);
        var items = query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new TemplateSearchResult(items, totalCount, pageNumber, pageSize,
            (pageNumber * pageSize) < totalCount);

        return Task.FromResult(result);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        _templates.TryRemove(id, out _);
        _versions.TryRemove(id, out _);
        _localizations.TryRemove(id, out _);
        _attachments.TryRemove(id, out _);
        await _cache.RemoveAsync(CacheKeys.TemplateKey(id));
        await _cache.RemoveAsync(CacheKeys.TemplateRenderKey(id, null));

        try
        {
            var result = await _templateService.ArchiveAsync(id, ct);
            return result.IsSuccess && result.Value;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to delete template {TemplateId} via ITemplateService", id);
            return true;
        }
    }

    public async Task<TemplateDetailDto> PublishAsync(Guid id, string? publishedBy, CancellationToken ct = default)
    {
        if (!_templates.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Template {id} not found");

        var now = DateTime.UtcNow;
        var updated = existing with
        {
            Status = TemplateStatus.Published,
            PublishedAt = now,
            PublishedBy = publishedBy,
            IsActive = true
        };

        _templates[id] = updated;

        UpdateVersionStatus(id, TemplateStatus.Published, publishedBy, now);

        try
        {
            var result = await _templateService.PublishAsync(id, ct);
            if (!result.IsSuccess)
                _logger.LogWarning("ITemplateService.PublishAsync failed for {TemplateId}: {Error}", id, result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to publish template {TemplateId} via ITemplateService", id);
        }

        await _cache.SetAsync(CacheKeys.TemplateKey(id), updated, CacheDuration);
        _logger.LogInformation("Published template {TemplateId}", id);
        return updated;
    }

    public async Task<TemplateDetailDto> ArchiveAsync(Guid id, CancellationToken ct = default)
    {
        if (!_templates.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Template {id} not found");

        var now = DateTime.UtcNow;
        var updated = existing with
        {
            Status = TemplateStatus.Archived,
            ArchivedAt = now,
            IsActive = false
        };

        _templates[id] = updated;

        try
        {
            var result = await _templateService.ArchiveAsync(id, ct);
            if (!result.IsSuccess)
                _logger.LogWarning("ITemplateService.ArchiveAsync failed for {TemplateId}: {Error}", id, result.Error);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to archive template {TemplateId} via ITemplateService", id);
        }

        await _cache.SetAsync(CacheKeys.TemplateKey(id), updated, CacheDuration);
        _logger.LogInformation("Archived template {TemplateId}", id);
        return updated;
    }

    public async Task<TemplateDetailDto> DraftAsync(Guid id, CancellationToken ct = default)
    {
        if (!_templates.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Template {id} not found");

        var updated = existing with
        {
            Status = TemplateStatus.Draft,
            PublishedAt = null,
            PublishedBy = null,
            ArchivedAt = null,
            IsActive = true
        };

        _templates[id] = updated;
        UpdateVersionStatus(id, TemplateStatus.Draft, null, DateTime.UtcNow);
        await _cache.SetAsync(CacheKeys.TemplateKey(id), updated, CacheDuration);
        _logger.LogInformation("Moved template {TemplateId} back to Draft", id);
        return updated;
    }

    public async Task<TemplateDetailDto> CloneAsync(Guid id, CloneTemplateRequest request, CancellationToken ct = default)
    {
        if (!_templates.TryGetValue(id, out var source))
            throw new KeyNotFoundException($"Source template {id} not found");

        var newId = Guid.NewGuid();
        var now = DateTime.UtcNow;
        var versionNumber = Interlocked.Increment(ref _nextVersionNumber);

        var variables = request.IncludeVariables
            ? source.Variables.Select(v => v with { Id = Guid.NewGuid() }).ToList()
            : new List<TemplateVariableDetailDto>();

        var localizations = request.IncludeLocalizations
            ? source.Localizations.Select(l => l with { Id = Guid.NewGuid(), CreatedAt = now }).ToList()
            : new List<TemplateLocalizationDto>();

        var partials = request.IncludePartials
            ? source.Partials.Select(p => p with { Id = Guid.NewGuid(), CreatedAt = now }).ToList()
            : new List<TemplatePartialDto>();

        var version = new TemplateVersionDetailDto(
            Guid.NewGuid(), versionNumber, source.SubjectTemplate, source.BodyTemplate,
            $"Cloned from {source.Name} ({source.Id})", TemplateStatus.Draft, null, now, null);

        var cloned = new TemplateDetailDto(
            newId, request.NewName, request.NewDescription ?? source.Description,
            source.ChannelType, request.NewCategory ?? source.Category,
            TemplateStatus.Draft, source.SubjectTemplate, source.BodyTemplate, true,
            versionNumber, now, null, null, null, null,
            new List<TemplateVersionDetailDto> { version },
            variables, localizations, partials,
            source.Attachments.Select(a => a with { Id = Guid.NewGuid() }).ToList(),
            source.Metadata);

        _templates[newId] = cloned;
        _versions[newId] = new List<TemplateVersionDetailDto> { version };

        if (localizations.Count > 0)
            _localizations[newId] = localizations;

        if (cloned.Attachments.Count > 0)
            _attachments[newId] = cloned.Attachments.Select(a => a with { Id = Guid.NewGuid() }).ToList();

        await _cache.SetAsync(CacheKeys.TemplateKey(newId), cloned, CacheDuration);
        _logger.LogInformation("Cloned template {SourceId} -> {TargetId} as '{Name}'", id, newId, request.NewName);
        return cloned;
    }

    public async Task<TemplateDetailDto> RollbackAsync(Guid id, RollbackTemplateRequest request, CancellationToken ct = default)
    {
        if (!_templates.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Template {id} not found");

        if (!_versions.TryGetValue(id, out var versionList))
            throw new InvalidOperationException($"No versions for template {id}");

        var targetVersion = versionList.FirstOrDefault(v => v.VersionNumber == request.TargetVersion)
            ?? throw new ArgumentException($"Version {request.TargetVersion} not found for template {id}");

        var now = DateTime.UtcNow;
        var newVersionNumber = Interlocked.Increment(ref _nextVersionNumber);

        var newVersion = new TemplateVersionDetailDto(
            Guid.NewGuid(), newVersionNumber, targetVersion.SubjectTemplate, targetVersion.BodyTemplate,
            request.ChangeNotes ?? $"Rollback to version {request.TargetVersion}",
            existing.Status, null, now, null);

        versionList.Add(newVersion);

        var updated = existing with
        {
            SubjectTemplate = targetVersion.SubjectTemplate,
            BodyTemplate = targetVersion.BodyTemplate,
            CurrentVersion = newVersionNumber,
            Versions = existing.Versions.Append(newVersion).ToList()
        };

        _templates[id] = updated;
        await _cache.SetAsync(CacheKeys.TemplateKey(id), updated, CacheDuration);
        _logger.LogInformation("Rolled back template {TemplateId} to version {Version}", id, request.TargetVersion);
        return updated;
    }

    public async Task<TemplateVersionDetailDto> CreateVersionAsync(Guid templateId, CreateTemplateVersionRequest request, CancellationToken ct = default)
    {
        if (!_templates.TryGetValue(templateId, out var template))
            throw new KeyNotFoundException($"Template {templateId} not found");

        var versionNumber = Interlocked.Increment(ref _nextVersionNumber);
        var now = DateTime.UtcNow;

        var version = new TemplateVersionDetailDto(
            Guid.NewGuid(), versionNumber, request.SubjectTemplate, request.BodyTemplate,
            request.ChangeNotes, template.Status, null, now, null);

        _versions.AddOrUpdate(templateId,
            _ => new List<TemplateVersionDetailDto> { version },
            (_, list) => { list.Add(version); return list; });

        var updated = template with
        {
            SubjectTemplate = request.SubjectTemplate,
            BodyTemplate = request.BodyTemplate,
            CurrentVersion = versionNumber,
            Versions = template.Versions.Append(version).ToList()
        };

        _templates[templateId] = updated;
        await _cache.SetAsync(CacheKeys.TemplateKey(templateId), updated, CacheDuration);

        try
        {
            await _templateService.CreateVersionAsync(
                new CreateTemplateVersionRequest(templateId, request.SubjectTemplate, request.BodyTemplate, request.ChangeNotes),
                ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to persist version for template {TemplateId}", templateId);
        }

        _logger.LogInformation("Created version {Version} for template {TemplateId}", versionNumber, templateId);
        return version;
    }

    public Task<List<TemplateVersionDetailDto>> GetVersionsAsync(Guid templateId, CancellationToken ct = default)
    {
        if (_versions.TryGetValue(templateId, out var versions))
            return Task.FromResult(versions.OrderByDescending(v => v.VersionNumber).ToList());

        if (_templates.ContainsKey(templateId))
            return Task.FromResult(_templates[templateId].Versions.OrderByDescending(v => v.VersionNumber).ToList());

        throw new KeyNotFoundException($"Template {templateId} not found");
    }

    public Task<TemplateVersionCompareDto> CompareVersionsAsync(Guid templateId, int fromVersion, int toVersion, CancellationToken ct = default)
    {
        if (!_versions.TryGetValue(templateId, out var versionList))
            throw new KeyNotFoundException($"No versions for template {templateId}");

        var from = versionList.FirstOrDefault(v => v.VersionNumber == fromVersion)
            ?? throw new ArgumentException($"Version {fromVersion} not found");
        var to = versionList.FirstOrDefault(v => v.VersionNumber == toVersion)
            ?? throw new ArgumentException($"Version {toVersion} not found");

        var changedFields = new List<string>();
        if (from.SubjectTemplate != to.SubjectTemplate) changedFields.Add("SubjectTemplate");
        if (from.BodyTemplate != to.BodyTemplate) changedFields.Add("BodyTemplate");

        var subjectDiff = ComputeDiff(from.SubjectTemplate, to.SubjectTemplate);
        var bodyDiff = ComputeDiff(from.BodyTemplate, to.BodyTemplate);

        var result = new TemplateVersionCompareDto(fromVersion, toVersion, subjectDiff, bodyDiff, changedFields);
        return Task.FromResult(result);
    }

    public async Task<TemplateRenderPreviewResult> PreviewAsync(TemplateRenderPreviewRequest request, CancellationToken ct = default)
    {
        var sw = Stopwatch.StartNew();
        var allVariables = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var unresolved = new List<string>();
        var warnings = new List<string>();

        ExtractTemplateVariables(request.SubjectTemplate, allVariables);
        ExtractTemplateVariables(request.BodyTemplate, allVariables);

        var renderData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        foreach (var kvp in request.TestData)
        {
            if (kvp.Value != null)
                renderData[kvp.Key] = kvp.Value;
        }

        string renderedSubject;
        string renderedBody;

        renderedSubject = RenderTemplateVariables(request.SubjectTemplate, renderData);
        renderedBody = RenderTemplateVariables(request.BodyTemplate, renderData);

        sw.Stop();

        foreach (var variable in allVariables)
        {
            if (!renderData.ContainsKey(variable))
                unresolved.Add(variable);
        }

        if (unresolved.Count > 0)
            warnings.Add($"Unresolved variables: {string.Join(", ", unresolved)}");

        return new TemplateRenderPreviewResult(
            renderedSubject, renderedBody, sw.ElapsedMilliseconds,
            renderData.Keys.ToList(), unresolved, warnings);
    }

    public async Task<TemplateRenderPreviewResult> PreviewTemplateAsync(Guid templateId, Dictionary<string, object?> testData, string? locale, CancellationToken ct = default)
    {
        var template = await GetByIdAsync(templateId, ct);
        string subjectTemplate = template.SubjectTemplate;
        string bodyTemplate = template.BodyTemplate;

        if (!string.IsNullOrWhiteSpace(locale))
        {
            var loc = template.Localizations.FirstOrDefault(l =>
                string.Equals(l.Locale, locale, StringComparison.OrdinalIgnoreCase));
            if (loc != null)
            {
                subjectTemplate = loc.SubjectTemplate ?? subjectTemplate;
                bodyTemplate = loc.BodyTemplate ?? bodyTemplate;
            }
        }

        var previewRequest = new TemplateRenderPreviewRequest(subjectTemplate, bodyTemplate, testData, locale);
        return await PreviewAsync(previewRequest, ct);
    }

    public async Task<TemplateLocalizationDto> AddLocalizationAsync(Guid templateId, CreateLocalizationRequest request, CancellationToken ct = default)
    {
        if (!_templates.ContainsKey(templateId))
            throw new KeyNotFoundException($"Template {templateId} not found");

        var now = DateTime.UtcNow;
        var dto = new TemplateLocalizationDto(
            Guid.NewGuid(), request.Locale, request.SubjectTemplate, request.BodyTemplate,
            !string.IsNullOrWhiteSpace(request.SubjectTemplate) && !string.IsNullOrWhiteSpace(request.BodyTemplate),
            now);

        _localizations.AddOrUpdate(templateId,
            _ => new List<TemplateLocalizationDto> { dto },
            (_, list) =>
            {
                list.RemoveAll(l => string.Equals(l.Locale, request.Locale, StringComparison.OrdinalIgnoreCase));
                list.Add(dto);
                return list;
            });

        if (_templates.TryGetValue(templateId, out var template))
        {
            var locs = template.Localizations
                .Where(l => !string.Equals(l.Locale, request.Locale, StringComparison.OrdinalIgnoreCase))
                .Append(dto)
                .ToList();

            _templates[templateId] = template with { Localizations = locs };
            await _cache.SetAsync(CacheKeys.TemplateKey(templateId), _templates[templateId], CacheDuration);
        }

        await _cache.RemoveAsync(CacheKeys.TemplateRenderKey(templateId, request.Locale));
        _logger.LogInformation("Added localization '{Locale}' to template {TemplateId}", request.Locale, templateId);
        return dto;
    }

    public Task<List<TemplateLocalizationDto>> GetLocalizationsAsync(Guid templateId, CancellationToken ct = default)
    {
        if (_localizations.TryGetValue(templateId, out var locs))
            return Task.FromResult(locs.ToList());

        if (_templates.TryGetValue(templateId, out var template))
            return Task.FromResult(template.Localizations.ToList());

        throw new KeyNotFoundException($"Template {templateId} not found");
    }

    public async Task<TemplateAttachmentMetaDto> AddAttachmentMetaAsync(Guid templateId, CreateAttachmentMetaRequest request, CancellationToken ct = default)
    {
        if (!_templates.ContainsKey(templateId))
            throw new KeyNotFoundException($"Template {templateId} not found");

        var dto = new TemplateAttachmentMetaDto(
            Guid.NewGuid(), request.FileName, request.ContentType,
            request.SizeBytes, request.IsRequired, request.Description);

        _attachments.AddOrUpdate(templateId,
            _ => new List<TemplateAttachmentMetaDto> { dto },
            (_, list) => { list.Add(dto); return list; });

        if (_templates.TryGetValue(templateId, out var template))
        {
            _templates[templateId] = template with
            {
                Attachments = template.Attachments.Append(dto).ToList()
            };
            await _cache.SetAsync(CacheKeys.TemplateKey(templateId), _templates[templateId], CacheDuration);
        }

        _logger.LogInformation("Added attachment '{FileName}' to template {TemplateId}", request.FileName, templateId);
        return dto;
    }

    public async Task<bool> RemoveAttachmentMetaAsync(Guid templateId, Guid attachmentId, CancellationToken ct = default)
    {
        if (!_templates.ContainsKey(templateId))
            throw new KeyNotFoundException($"Template {templateId} not found");

        if (_attachments.TryGetValue(templateId, out var list))
        {
            var removed = list.RemoveAll(a => a.Id == attachmentId);
            if (removed > 0 && _templates.TryGetValue(templateId, out var template))
            {
                _templates[templateId] = template with
                {
                    Attachments = template.Attachments.Where(a => a.Id != attachmentId).ToList()
                };
                await _cache.SetAsync(CacheKeys.TemplateKey(templateId), _templates[templateId], CacheDuration);
                _logger.LogInformation("Removed attachment {AttachmentId} from template {TemplateId}", attachmentId, templateId);
                return true;
            }
        }

        return false;
    }

    public async Task<TemplateDetailDto> UpdateCategoryAsync(Guid id, TemplateCategory category, CancellationToken ct = default)
    {
        if (!_templates.TryGetValue(id, out var existing))
            throw new KeyNotFoundException($"Template {id} not found");

        var updated = existing with { Category = category };
        _templates[id] = updated;
        await _cache.SetAsync(CacheKeys.TemplateKey(id), updated, CacheDuration);
        _logger.LogInformation("Updated template {TemplateId} category to {Category}", id, category);
        return updated;
    }

    public Task<List<TemplateCategory>> GetCategoriesAsync(CancellationToken ct = default)
    {
        var categories = Enum.GetValues<TemplateCategory>().ToList();
        return Task.FromResult(categories);
    }

    public Task<int> GetTemplateCountByStatusAsync(TemplateStatus status, CancellationToken ct = default)
    {
        var count = _templates.Values.Count(t => t.Status == status);
        return Task.FromResult(count);
    }

    public Task<Dictionary<TemplateCategory, int>> GetTemplateCountByCategoryAsync(CancellationToken ct = default)
    {
        var counts = _templates.Values
            .GroupBy(t => t.Category)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var cat in Enum.GetValues<TemplateCategory>())
        {
            if (!counts.ContainsKey(cat))
                counts[cat] = 0;
        }

        return Task.FromResult(counts);
    }

    private TemplatePartialDto? ResolvePartial(string partialName)
    {
        _partials.TryGetValue(partialName, out var partial);
        return partial;
    }

    private void UpdateVersionStatus(Guid templateId, TemplateStatus status, string? publishedBy, DateTime timestamp)
    {
        if (!_versions.TryGetValue(templateId, out var list))
            return;

        var latest = list.OrderByDescending(v => v.VersionNumber).FirstOrDefault();
        if (latest == null) return;

        var idx = list.IndexOf(latest);
        list[idx] = latest with
        {
            Status = status,
            PublishedBy = publishedBy,
            PublishedAt = status == TemplateStatus.Published ? timestamp : null
        };
    }

    private static string ComputeDiff(string from, string to)
    {
        if (from == to) return string.Empty;

        var fromLines = from.Split('\n');
        var toLines = to.Split('\n');
        var diff = new System.Text.StringBuilder();

        var maxLen = Math.Max(fromLines.Length, toLines.Length);
        for (int i = 0; i < maxLen; i++)
        {
            var f = i < fromLines.Length ? fromLines[i] : null;
            var t = i < toLines.Length ? toLines[i] : null;

            if (f == t) continue;

            if (f != null)
                diff.AppendLine($"-{f}");

            if (t != null)
                diff.AppendLine($"+{t}");
        }

        return diff.ToString().TrimEnd();
    }

    private static TemplateDetailDto MapFromTemplateDto(TemplateDto dto)
    {
        return new TemplateDetailDto(
            dto.Id,
            dto.Name,
            dto.Description,
            dto.ChannelType,
            TemplateCategory.General,
            TemplateStatus.Draft,
            dto.SubjectTemplate,
            dto.BodyTemplate,
            dto.IsActive,
            dto.CurrentVersion,
            dto.CreatedAt,
            null,
            null,
            null,
            null,
            dto.Versions.Select(v => new TemplateVersionDetailDto(
                v.Id, v.VersionNumber, v.SubjectTemplate, v.BodyTemplate,
                v.ChangeNotes, TemplateStatus.Draft, null, v.PublishedAt, v.PublishedAt)).ToList(),
            dto.Variables.Select(v => new TemplateVariableDetailDto(
                v.Id, v.Name, v.Description, v.IsRequired,
                v.DefaultValue, v.DataType, null, null, null, null)).ToList(),
            new List<TemplateLocalizationDto>(),
            new List<TemplatePartialDto>(),
            new List<TemplateAttachmentMetaDto>(),
            null);
    }

    private static string RenderTemplateVariables(string template, Dictionary<string, object> data)
    {
        return Regex.Replace(template, @"\{\{(\w+(?:\.\w+)*)\}\}", match =>
        {
            var key = match.Groups[1].Value;
            var value = ResolveNestedValue(key, data);
            return value?.ToString() ?? match.Value;
        });
    }

    private static object? ResolveNestedValue(string path, Dictionary<string, object> data)
    {
        var parts = path.Split('.');
        object? current = data;
        foreach (var part in parts)
        {
            if (current is Dictionary<string, object> dict)
            {
                if (dict.TryGetValue(part, out var val))
                    current = val;
                else
                    return null;
            }
            else
            {
                var prop = current?.GetType().GetProperty(part);
                if (prop != null)
                    current = prop.GetValue(current);
                else
                    return null;
            }
        }
        return current;
    }

    private static void ExtractTemplateVariables(string template, HashSet<string> variables)
    {
        var matches = Regex.Matches(template, @"\{\{(\w+(?:\.\w+)*)\}\}");
        foreach (Match match in matches)
            variables.Add(match.Groups[1].Value.Split('.')[0]);
    }

    private void SeedSamplePartials()
    {
        _partials.TryAdd("header", new TemplatePartialDto(
            Guid.NewGuid(), "header", "<div class=\"header\">{{title}}</div>",
            "Standard email header", null, new Dictionary<string, string> { { "title", "string" } }, DateTime.UtcNow));
        _partials.TryAdd("footer", new TemplatePartialDto(
            Guid.NewGuid(), "footer", "<div class=\"footer\">{{year}} SportsGurukul. All rights reserved.</div>",
            "Standard email footer", null, new Dictionary<string, string> { { "year", "number" } }, DateTime.UtcNow));
        _partials.TryAdd("button", new TemplatePartialDto(
            Guid.NewGuid(), "button", "<a href=\"{{url}}\" class=\"btn\">{{label}}</a>",
            "Standard action button", null, new Dictionary<string, string> { { "url", "string" }, { "label", "string" } }, DateTime.UtcNow));
    }

    private static class CacheKeys
    {
        public const string TemplatePrefix = "templates:";
        public static string TemplateKey(Guid id) => $"{TemplatePrefix}{id}";
        public static string TemplateRenderKey(Guid id, string? locale) => $"{TemplatePrefix}render:{id}:{locale ?? "default"}";
    }
}
