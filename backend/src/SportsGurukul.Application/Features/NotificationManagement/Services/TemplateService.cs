using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.BusinessRules;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;
using SportsGurukul.Domain.Entities.Notification;

namespace SportsGurukul.Application.Features.NotificationManagement.Services;

public class TemplateService : ITemplateService
{
    private readonly ITemplateRepository _templateRepository;
    private readonly IBusinessRuleValidator _ruleValidator;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ILogger<TemplateService> _logger;

    public TemplateService(
        ITemplateRepository templateRepository,
        IBusinessRuleValidator ruleValidator,
        ITemplateRenderer templateRenderer,
        ILogger<TemplateService> logger)
    {
        _templateRepository = templateRepository;
        _ruleValidator = ruleValidator;
        _templateRenderer = templateRenderer;
        _logger = logger;
    }

    public async Task<Result<TemplateDto>> CreateAsync(CreateTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var validation = await _ruleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsSuccess)
            return Result<TemplateDto>.Failure(validation.Errors);

        var existing = await _templateRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existing is not null)
            return Result<TemplateDto>.Failure($"Template with name '{request.Name}' already exists");

        var variables = request.Variables?.Select(v => new TemplateVariable
        {
            Id = Guid.NewGuid(),
            Name = v.Name,
            Description = v.Description,
            IsRequired = v.IsRequired,
            DefaultValue = v.DefaultValue,
            DataType = v.DataType
        }).ToList() ?? [];

        var entity = new NotificationTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            ChannelType = request.ChannelType,
            SubjectTemplate = request.SubjectTemplate,
            BodyTemplate = request.BodyTemplate,
            IsActive = true,
            CurrentVersion = 1,
            CreatedAt = DateTime.UtcNow,
            Variables = variables
        };

        entity.Versions.Add(new TemplateVersion
        {
            Id = Guid.NewGuid(),
            TemplateId = entity.Id,
            VersionNumber = 1,
            SubjectTemplate = request.SubjectTemplate,
            BodyTemplate = request.BodyTemplate,
            ChangeNotes = "Initial version",
            PublishedAt = DateTime.UtcNow
        });

        foreach (var variable in variables)
            variable.TemplateId = entity.Id;

        await _templateRepository.AddAsync(entity, cancellationToken);
        _logger.LogInformation("Created template {TemplateId} with name {TemplateName}", entity.Id, entity.Name);

        return Result<TemplateDto>.Success(MapToDto(entity));
    }

    public async Task<Result<TemplateDto>> UpdateAsync(UpdateTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetWithVersionsAsync(request.Id, cancellationToken);
        if (entity is null)
            return Result<TemplateDto>.Failure($"Template {request.Id} not found");

        var validation = await _ruleValidator.ValidateAsync(request, cancellationToken);
        if (!validation.IsSuccess)
            return Result<TemplateDto>.Failure(validation.Errors);

        if (request.Name is not null) entity.Name = request.Name;
        if (request.Description is not null) entity.Description = request.Description;
        if (request.SubjectTemplate is not null) entity.SubjectTemplate = request.SubjectTemplate;
        if (request.BodyTemplate is not null) entity.BodyTemplate = request.BodyTemplate;
        entity.UpdatedAt = DateTime.UtcNow;

        _templateRepository.Update(entity);
        _logger.LogInformation("Updated template {TemplateId}", entity.Id);

        return Result<TemplateDto>.Success(MapToDto(entity));
    }

    public async Task<Result<TemplateDto>> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetWithVersionsAsync(id, cancellationToken);
        if (entity is null)
            return Result<TemplateDto>.Failure($"Template {id} not found");

        entity.CurrentVersion++;
        entity.IsActive = true;

        entity.Versions.Add(new TemplateVersion
        {
            Id = Guid.NewGuid(),
            TemplateId = entity.Id,
            VersionNumber = entity.CurrentVersion,
            SubjectTemplate = entity.SubjectTemplate,
            BodyTemplate = entity.BodyTemplate,
            ChangeNotes = $"Published version {entity.CurrentVersion}",
            PublishedAt = DateTime.UtcNow
        });

        entity.UpdatedAt = DateTime.UtcNow;
        _templateRepository.Update(entity);
        _logger.LogInformation("Published template {TemplateId} version {Version}", entity.Id, entity.CurrentVersion);

        return Result<TemplateDto>.Success(MapToDto(entity));
    }

    public async Task<Result<bool>> ArchiveAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetByIdAsync(id, cancellationToken);
        if (entity is null)
            return Result<bool>.Failure($"Template {id} not found");

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        _templateRepository.Update(entity);
        _logger.LogInformation("Archived template {TemplateId}", id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<TemplateVersionDto>> CreateVersionAsync(CreateTemplateVersionRequest request, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetWithVersionsAsync(request.TemplateId, cancellationToken);
        if (entity is null)
            return Result<TemplateVersionDto>.Failure($"Template {request.TemplateId} not found");

        var version = new TemplateVersion
        {
            Id = Guid.NewGuid(),
            TemplateId = entity.Id,
            VersionNumber = entity.CurrentVersion + 1,
            SubjectTemplate = request.SubjectTemplate,
            BodyTemplate = request.BodyTemplate,
            ChangeNotes = request.ChangeNotes,
            PublishedAt = DateTime.UtcNow
        };

        entity.CurrentVersion++;
        entity.SubjectTemplate = request.SubjectTemplate;
        entity.BodyTemplate = request.BodyTemplate;
        entity.Versions.Add(version);
        entity.UpdatedAt = DateTime.UtcNow;

        _templateRepository.Update(entity);
        _logger.LogInformation("Created template {TemplateId} version {Version}", entity.Id, version.VersionNumber);

        return Result<TemplateVersionDto>.Success(MapToVersionDto(version));
    }

    public async Task<Result<TemplateDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetWithVersionsAsync(id, cancellationToken);
        if (entity is null)
            return Result<TemplateDto>.Failure($"Template {id} not found");

        return Result<TemplateDto>.Success(MapToDto(entity));
    }

    public async Task<Result<List<TemplateVersionDto>>> GetVersionsAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetWithVersionsAsync(templateId, cancellationToken);
        if (entity is null)
            return Result<List<TemplateVersionDto>>.Failure($"Template {templateId} not found");

        var versions = entity.Versions
            .OrderByDescending(v => v.VersionNumber)
            .Select(MapToVersionDto)
            .ToList();

        return Result<List<TemplateVersionDto>>.Success(versions);
    }

    private static TemplateDto MapToDto(NotificationTemplate entity)
    {
        var versions = entity.Versions?
            .OrderByDescending(v => v.VersionNumber)
            .Select(MapToVersionDto)
            .ToList() ?? [];

        var variables = entity.Variables?
            .Select(v => new TemplateVariableDto(
                v.Id, v.Name, v.Description, v.IsRequired, v.DefaultValue, v.DataType))
            .ToList() ?? [];

        return new TemplateDto(
            entity.Id, entity.Name, entity.Description,
            entity.ChannelType, entity.SubjectTemplate, entity.BodyTemplate,
            entity.IsActive, entity.CurrentVersion, entity.CreatedAt,
            versions, variables);
    }

    private static TemplateVersionDto MapToVersionDto(TemplateVersion v) =>
        new(v.Id, v.VersionNumber, v.SubjectTemplate, v.BodyTemplate, v.ChangeNotes, v.PublishedAt);
}
