using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.Notification;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Application.Features.NotificationManagement.DTOs;

using SportsGurukul.Application.Features.NotificationManagement.BusinessRules.Rules;

namespace SportsGurukul.Application.Features.NotificationManagement.BusinessRules;

public class TemplateValidationRule : IBusinessRule
{
    private readonly ITemplateRepository _templateRepository;
    private readonly ITemplateRenderer _templateRenderer;
    private readonly ILogger<TemplateValidationRule> _logger;

    public TemplateValidationRule(
        ITemplateRepository templateRepository,
        ITemplateRenderer templateRenderer,
        ILogger<TemplateValidationRule> logger)
    {
        _templateRepository = templateRepository;
        _templateRenderer = templateRenderer;
        _logger = logger;
    }

    public async Task<Result<bool>> ValidateAsync<T>(T request, CancellationToken cancellationToken = default)
    {
        string? subjectTemplate = null;
        string? bodyTemplate = null;

        if (request is CreateNotificationRequest createRequest && createRequest.TemplateId.HasValue)
        {
            var template = await _templateRepository
                .GetByIdAsync(createRequest.TemplateId.Value, cancellationToken);
            if (template is null)
                return Result<bool>.Failure($"Template {createRequest.TemplateId} not found");
            subjectTemplate = template.SubjectTemplate;
            bodyTemplate = template.BodyTemplate;
        }

        if (request is CreateTemplateVersionRequest versionRequest)
        {
            subjectTemplate = versionRequest.SubjectTemplate;
            bodyTemplate = versionRequest.BodyTemplate;
        }

        if (request is CreateTemplateRequest templateRequest)
        {
            subjectTemplate = templateRequest.SubjectTemplate;
            bodyTemplate = templateRequest.BodyTemplate;
        }

        if (subjectTemplate is null || bodyTemplate is null)
            return Result<bool>.Success(true);

        var subjectVars = _templateRenderer.ExtractVariables(subjectTemplate);
        var bodyVars = _templateRenderer.ExtractVariables(bodyTemplate);

        var allVars = subjectVars.Concat(bodyVars).Distinct().ToList();
        if (allVars.Count > 0)
        {
            _logger.LogInformation("Template contains {VariableCount} variables: {Variables}",
                allVars.Count, string.Join(", ", allVars));
        }

        return Result<bool>.Success(true);
    }
}
