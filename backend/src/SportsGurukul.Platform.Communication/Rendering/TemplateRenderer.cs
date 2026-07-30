using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportsGurukul.Application.Common.Interfaces.Notification.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Platform.Communication.Configuration;

namespace SportsGurukul.Platform.Communication.Rendering;

public class TemplateRenderer : ITemplateRenderer
{
    private readonly ITemplateEngine _engine;
    private readonly VariableResolver _variableResolver;
    private readonly LocalizedTemplateEngine _localizedEngine;
    private readonly TemplateEngineOptions _options;
    private readonly ILogger<TemplateRenderer> _logger;

    public TemplateRenderer(
        ITemplateEngine engine,
        VariableResolver variableResolver,
        LocalizedTemplateEngine localizedEngine,
        IOptions<CommunicationOptions> options,
        ILogger<TemplateRenderer> logger)
    {
        _engine = engine;
        _variableResolver = variableResolver;
        _localizedEngine = localizedEngine;
        _options = options.Value.TemplateEngine;
        _logger = logger;
    }

    public async Task<Result<(string Subject, string Body)>> RenderAsync(
        string subjectTemplate,
        string bodyTemplate,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var resolvedVariables = _variableResolver.Resolve(variables);

            string subject;
            string body;

            if (_options.EnableLocalization)
            {
                subject = await _localizedEngine.RenderLocalizedAsync(subjectTemplate, resolvedVariables, _options.DefaultLocale);
                body = await _localizedEngine.RenderLocalizedAsync(bodyTemplate, resolvedVariables, _options.DefaultLocale);
            }
            else
            {
                subject = await _engine.RenderAsync(subjectTemplate, resolvedVariables);
                body = await _engine.RenderAsync(bodyTemplate, resolvedVariables);
            }

            if (_options.StrictMode)
            {
                var unrendered = ExtractUnrenderedVariables(subject);
                unrendered.AddRange(ExtractUnrenderedVariables(body));

                if (unrendered.Count > 0)
                {
                    _logger.LogWarning("Strict mode: unrendered variables in template: {Variables}",
                        string.Join(", ", unrendered));
                }
            }

            return Result<(string, string)>.Success((subject, body));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Template rendering failed");
            return Result<(string, string)>.Failure($"Template rendering failed: {ex.Message}");
        }
    }

    public IReadOnlyList<string> ExtractVariables(string template)
    {
        return _engine.ExtractVariables(template);
    }

    private static List<string> ExtractUnrenderedVariables(string text)
    {
        var result = new List<string>();
        var matches = Regex.Matches(text, @"\{\{\s*(\w+)\s*\}\}");

        foreach (Match match in matches)
        {
            var varName = match.Groups[1].Value;
            if (!string.IsNullOrEmpty(varName) && !result.Contains(varName))
                result.Add(varName);
        }

        return result;
    }
}
