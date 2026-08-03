using MediatR;
using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.AIManagement.DTOs;
using SportsGurukul.Application.Features.AIManagement.Events;
using SportsGurukul.Domain.Entities.AI;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class PromptService : IPromptService
{
    private readonly IPromptRepository _promptRepository;
    private readonly IAssistantRepository _assistantRepository;
    private readonly IRepository<PromptVersion> _versionRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;
    private readonly ILogger<PromptService> _logger;

    public PromptService(
        IPromptRepository promptRepository,
        IAssistantRepository assistantRepository,
        IRepository<PromptVersion> versionRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator,
        ILogger<PromptService> logger)
    {
        _promptRepository = promptRepository;
        _assistantRepository = assistantRepository;
        _versionRepository = versionRepository;
        _unitOfWork = unitOfWork;
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<Result<PromptTemplateDto>> CreateAsync(CreatePromptTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var assistant = await _assistantRepository.GetByIdAsync(request.AssistantId, cancellationToken);
        if (assistant is null)
            return Result<PromptTemplateDto>.Failure("Assistant not found");

        var template = new PromptTemplate
        {
            AssistantId = request.AssistantId,
            Name = request.Name,
            Description = request.Description,
            PromptType = request.PromptType,
            TemplateText = request.TemplateText,
            InputSchemaJson = request.InputSchemaJson,
            OutputSchemaJson = request.OutputSchemaJson,
            VariablesJson = request.VariablesJson,
            CurrentVersion = 1,
            IsActive = true,
            IsDefault = request.IsDefault,
        };

        if (request.IsDefault)
        {
            var existingDefaults = await _promptRepository.GetActiveByAssistantAsync(request.AssistantId, cancellationToken);
            foreach (var existing in existingDefaults.Where(t => t.IsDefault))
            {
                existing.IsDefault = false;
                _promptRepository.Update(existing);
            }
        }

        await _promptRepository.AddAsync(template, cancellationToken);

        var version = new PromptVersion
        {
            PromptTemplateId = template.Id,
            VersionNumber = 1,
            Content = request.TemplateText,
            ChangeSummary = "Initial version",
            IsActive = true,
            DeployedAt = DateTime.UtcNow,
        };
        await _versionRepository.AddAsync(version, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Prompt template created: {PromptName} for assistant {AssistantId}", template.Name, template.AssistantId);
        return Result<PromptTemplateDto>.Success(MapToDto(template));
    }

    public async Task<Result<PromptTemplateDto>> UpdateAsync(UpdatePromptTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = await _promptRepository.GetByIdWithVersionsAsync(request.PromptTemplateId, cancellationToken);
        if (template is null)
            return Result<PromptTemplateDto>.Failure("Prompt template not found");

        if (request.ExpectedRowVersion is { Length: > 0 } && !template.RowVersion.SequenceEqual(request.ExpectedRowVersion))
            return Result<PromptTemplateDto>.Failure("The prompt template was modified by another user; please refresh and try again");

        if (!string.IsNullOrWhiteSpace(request.Name)) template.Name = request.Name;
        if (request.Description is not null) template.Description = request.Description;
        if (request.TemplateText is not null) template.TemplateText = request.TemplateText;
        if (request.InputSchemaJson is not null) template.InputSchemaJson = request.InputSchemaJson;
        if (request.OutputSchemaJson is not null) template.OutputSchemaJson = request.OutputSchemaJson;
        if (request.VariablesJson is not null) template.VariablesJson = request.VariablesJson;
        if (request.IsActive.HasValue) template.IsActive = request.IsActive.Value;

        template.UpdatedAt = DateTime.UtcNow;
        _promptRepository.Update(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PromptTemplateDto>.Success(MapToDto(template));
    }

    public async Task<Result<PromptTemplateDto>> PublishAsync(PublishPromptTemplateRequest request, CancellationToken cancellationToken = default)
    {
        var template = await _promptRepository.GetByIdWithVersionsAsync(request.PromptTemplateId, cancellationToken);
        if (template is null)
            return Result<PromptTemplateDto>.Failure("Prompt template not found");

        var nextVersion = template.CurrentVersion + 1;
        var version = new PromptVersion
        {
            PromptTemplateId = template.Id,
            VersionNumber = nextVersion,
            Content = template.TemplateText,
            ChangeSummary = request.ChangeSummary,
            Notes = request.Notes,
            IsActive = true,
            DeployedAt = DateTime.UtcNow,
        };
        await _versionRepository.AddAsync(version, cancellationToken);

        template.CurrentVersion = nextVersion;
        template.UpdatedAt = DateTime.UtcNow;
        _promptRepository.Update(template);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Publish(
            new PromptPublishedEvent(template.Id, template.AssistantId, template.Name, nextVersion, DateTime.UtcNow),
            cancellationToken);

        return Result<PromptTemplateDto>.Success(MapToDto(template));
    }

    public async Task<Result<PromptTemplateDto>> RollbackAsync(RollbackPromptVersionRequest request, CancellationToken cancellationToken = default)
    {
        var template = await _promptRepository.GetByIdWithVersionsAsync(request.PromptTemplateId, cancellationToken);
        if (template is null)
            return Result<PromptTemplateDto>.Failure("Prompt template not found");

        var version = template.Versions.FirstOrDefault(v => v.VersionNumber == request.VersionNumber);
        if (version is null)
            return Result<PromptTemplateDto>.Failure($"Version {request.VersionNumber} does not exist");

        template.TemplateText = version.Content;
        template.CurrentVersion = version.VersionNumber;
        template.UpdatedAt = DateTime.UtcNow;

        foreach (var existing in template.Versions)
            existing.IsActive = existing.Id == version.Id;

        _promptRepository.Update(template);
        foreach (var existing in template.Versions)
            _versionRepository.Update(existing);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PromptTemplateDto>.Success(MapToDto(template));
    }

    public async Task<Result<PromptTemplateDto>> CloneAsync(ClonePromptRequest request, CancellationToken cancellationToken = default)
    {
        var source = await _promptRepository.GetByIdWithVersionsAsync(request.SourcePromptId, cancellationToken);
        if (source is null)
            return Result<PromptTemplateDto>.Failure("Source prompt template not found");

        var targetAssistantId = request.TargetAssistantId ?? source.AssistantId;
        var assistant = await _assistantRepository.GetByIdAsync(targetAssistantId, cancellationToken);
        if (assistant is null)
            return Result<PromptTemplateDto>.Failure("Target assistant not found");

        var clone = new PromptTemplate
        {
            AssistantId = targetAssistantId,
            Name = request.NewName ?? $"{source.Name} (Clone)",
            Description = source.Description,
            PromptType = source.PromptType,
            TemplateText = source.TemplateText,
            InputSchemaJson = source.InputSchemaJson,
            OutputSchemaJson = source.OutputSchemaJson,
            VariablesJson = source.VariablesJson,
            CurrentVersion = 1,
            IsActive = false,
            IsDefault = false,
        };

        await _promptRepository.AddAsync(clone, cancellationToken);

        var version = new PromptVersion
        {
            PromptTemplateId = clone.Id,
            VersionNumber = 1,
            Content = source.TemplateText,
            ChangeSummary = $"Cloned from prompt template {source.Id}",
            IsActive = true,
            DeployedAt = DateTime.UtcNow,
        };
        await _versionRepository.AddAsync(version, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<PromptTemplateDto>.Success(MapToDto(clone));
    }

    public async Task<Result<PromptTemplateDto>> GetByIdAsync(Guid promptTemplateId, CancellationToken cancellationToken = default)
    {
        var template = await _promptRepository.GetByIdWithVersionsAsync(promptTemplateId, cancellationToken);
        if (template is null)
            return Result<PromptTemplateDto>.Failure("Prompt template not found");

        return Result<PromptTemplateDto>.Success(MapToDto(template));
    }

    public async Task<Result<IReadOnlyList<PromptTemplateDto>>> SearchAsync(
        string? searchTerm,
        Guid? assistantId,
        AIPromptType? promptType,
        bool? isActive,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        IReadOnlyList<PromptTemplate> templates;
        if (assistantId.HasValue)
            templates = await _promptRepository.GetByAssistantIdAsync(assistantId.Value, cancellationToken);
        else if (promptType.HasValue)
            templates = await _promptRepository.GetByTypeAsync(promptType.Value, cancellationToken);
        else
            templates = await _promptRepository.GetAllAsync(cancellationToken);

        var query = templates.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(t =>
                t.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                (t.Description?.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ?? false));
        }

        if (isActive.HasValue)
            query = query.Where(t => t.IsActive == isActive.Value);

        var paged = query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return Result<IReadOnlyList<PromptTemplateDto>>.Success(paged.Select(MapToDto).ToList());
    }

    private static PromptTemplateDto MapToDto(PromptTemplate template)
    {
        var versions = (template.Versions ?? new List<PromptVersion>())
            .OrderBy(v => v.VersionNumber)
            .Select(v => new PromptVersionDto(
                v.Id,
                v.PromptTemplateId,
                v.VersionNumber,
                v.Content,
                v.ChangeSummary,
                v.Notes,
                v.CreatedByUserId,
                v.IsActive,
                v.DeployedAt,
                v.CreatedAt))
            .ToList();

        return new PromptTemplateDto(
            template.Id,
            template.AssistantId,
            template.Name,
            template.Description,
            template.PromptType,
            template.TemplateText,
            template.InputSchemaJson,
            template.OutputSchemaJson,
            template.VariablesJson,
            template.CurrentVersion,
            template.IsActive,
            template.IsDefault,
            versions,
            template.CreatedAt,
            template.UpdatedAt);
    }
}
