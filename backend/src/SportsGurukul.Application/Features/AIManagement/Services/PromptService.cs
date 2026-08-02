using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Models;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DomainEvents;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class PromptService : IPromptService
{
    private readonly IPromptTemplateRepository _templateRepository;
    private readonly IPromptVersionRepository _versionRepository;
    private readonly IPublisher _publisher;
    private readonly ILogger<PromptService> _logger;

    public PromptService(
        IPromptTemplateRepository templateRepository,
        IPromptVersionRepository versionRepository,
        IPublisher publisher,
        ILogger<PromptService> logger)
    {
        _templateRepository = templateRepository;
        _versionRepository = versionRepository;
        _publisher = publisher;
        _logger = logger;
    }

    public async Task<Result<PromptTemplate>> CreateAsync(CreatePromptTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            Type = request.Type,
            TemplateContent = request.TemplateContent,
            Variables = request.Variables,
            Tags = request.Tags,
            Category = request.Category,
            CurrentVersion = 1,
            Status = PromptStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        await _templateRepository.AddAsync(template, cancellationToken);

        var version = new PromptVersion
        {
            Id = Guid.NewGuid(),
            PromptTemplateId = template.Id,
            VersionNumber = 1,
            Content = request.TemplateContent,
            CreatedAt = DateTime.UtcNow
        };

        await _versionRepository.AddAsync(version, cancellationToken);

        _logger.LogInformation("Created prompt template {TemplateId} with name {Name}", template.Id, template.Name);

        return Result<PromptTemplate>.Success(template);
    }

    public async Task<Result<PromptTemplate>> UpdateAsync(UpdatePromptTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = await _templateRepository.GetByIdAsync(request.Id, cancellationToken);
        if (template is null || template.IsDeleted)
            return Result<PromptTemplate>.Failure("Prompt template not found");

        if (request.Name is not null) template.Name = request.Name;
        if (request.Description is not null) template.Description = request.Description;
        if (request.TemplateContent is not null) template.TemplateContent = request.TemplateContent;
        if (request.Variables is not null) template.Variables = request.Variables;
        if (request.Tags is not null) template.Tags = request.Tags;
        if (request.Category is not null) template.Category = request.Category;

        var newVersionNumber = template.CurrentVersion + 1;

        var version = new PromptVersion
        {
            Id = Guid.NewGuid(),
            PromptTemplateId = template.Id,
            VersionNumber = newVersionNumber,
            Content = request.TemplateContent ?? template.TemplateContent,
            CreatedAt = DateTime.UtcNow
        };

        await _versionRepository.AddAsync(version, cancellationToken);

        template.CurrentVersion = newVersionNumber;
        template.UpdatedAt = DateTime.UtcNow;

        _templateRepository.Update(template);

        _logger.LogInformation("Updated prompt template {TemplateId} to version {Version}", request.Id, newVersionNumber);

        return Result<PromptTemplate>.Success(template);
    }

    public async Task<Result<PromptTemplate>> PublishAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var template = await _templateRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (template is null || template.IsDeleted)
            return Result<PromptTemplate>.Failure("Prompt template not found");

        template.Status = PromptStatus.Active;
        template.UpdatedAt = DateTime.UtcNow;

        _templateRepository.Update(template);

        await _publisher.Publish(new PromptPublishedEvent(template.Id, template.Name, template.CurrentVersion, DateTime.UtcNow), cancellationToken);

        _logger.LogInformation("Published prompt template {TemplateId} as active", id);

        return Result<PromptTemplate>.Success(template);
    }

    public async Task<Result<PromptTemplate>> RollbackAsync(Guid id, int versionNumber, CancellationToken cancellationToken = default)
    {
        var template = await _templateRepository.GetByIdAsync(id, cancellationToken);
        if (template is null || template.IsDeleted)
            return Result<PromptTemplate>.Failure("Prompt template not found");

        var oldVersion = (await _versionRepository.GetByTemplateIdAsync(id, cancellationToken))
            .FirstOrDefault(v => v.VersionNumber == versionNumber);

        if (oldVersion is null)
            return Result<PromptTemplate>.Failure("Version not found");

        var newVersionNumber = template.CurrentVersion + 1;

        var newVersion = new PromptVersion
        {
            Id = Guid.NewGuid(),
            PromptTemplateId = id,
            VersionNumber = newVersionNumber,
            Content = oldVersion.Content,
            ChangeNotes = $"Rollback to version {versionNumber}",
            CreatedAt = DateTime.UtcNow
        };

        await _versionRepository.AddAsync(newVersion, cancellationToken);

        template.TemplateContent = oldVersion.Content;
        template.CurrentVersion = newVersionNumber;
        template.UpdatedAt = DateTime.UtcNow;

        _templateRepository.Update(template);

        _logger.LogInformation("Rolled back prompt template {TemplateId} to version {Version}", id, versionNumber);

        return Result<PromptTemplate>.Success(template);
    }

    public async Task<Result<PromptTemplate>> CloneAsync(Guid id, string newName, CancellationToken cancellationToken = default)
    {
        var source = await _templateRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (source is null || source.IsDeleted)
            return Result<PromptTemplate>.Failure("Prompt template not found");

        var clone = new PromptTemplate
        {
            Id = Guid.NewGuid(),
            Name = newName,
            Description = source.Description,
            Type = source.Type,
            TemplateContent = source.TemplateContent,
            Variables = source.Variables,
            Tags = source.Tags,
            Category = source.Category,
            CurrentVersion = 1,
            Status = PromptStatus.Draft,
            CreatedAt = DateTime.UtcNow
        };

        await _templateRepository.AddAsync(clone, cancellationToken);

        var version = new PromptVersion
        {
            Id = Guid.NewGuid(),
            PromptTemplateId = clone.Id,
            VersionNumber = 1,
            Content = source.TemplateContent,
            ChangeNotes = $"Cloned from template {id}",
            CreatedAt = DateTime.UtcNow
        };

        await _versionRepository.AddAsync(version, cancellationToken);

        _logger.LogInformation("Cloned prompt template {SourceId} to new template {TargetId} with name {Name}", id, clone.Id, newName);

        return Result<PromptTemplate>.Success(clone);
    }

    public async Task<Result<PromptTemplate>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await _templateRepository.GetByIdWithDetailsAsync(id, cancellationToken);
        if (entity is null || entity.IsDeleted)
            return Result<PromptTemplate>.Failure("Prompt template not found");

        return Result<PromptTemplate>.Success(entity);
    }

    public async Task<Result<IReadOnlyList<PromptVersion>>> GetVersionsAsync(Guid templateId, CancellationToken cancellationToken = default)
    {
        var versions = await _versionRepository.GetByTemplateIdAsync(templateId, cancellationToken);

        return Result<IReadOnlyList<PromptVersion>>.Success(versions);
    }

    public async Task<Result<IReadOnlyList<PromptTemplate>>> SearchAsync(SearchPromptsRequest request, CancellationToken cancellationToken = default)
    {
        var query = await _templateRepository.FindAsync(t =>
            !t.IsDeleted &&
            (string.IsNullOrEmpty(request.SearchTerm) || t.Name.Contains(request.SearchTerm) || (t.Description != null && t.Description.Contains(request.SearchTerm))) &&
            (!request.Type.HasValue || t.Type == request.Type) &&
            (!request.Status.HasValue || t.Status == request.Status) &&
            (string.IsNullOrEmpty(request.Category) || t.Category == request.Category), cancellationToken);

        return Result<IReadOnlyList<PromptTemplate>>.Success(query);
    }
}
