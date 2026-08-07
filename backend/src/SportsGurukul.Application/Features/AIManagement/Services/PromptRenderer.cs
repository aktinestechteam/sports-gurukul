using Microsoft.Extensions.Logging;
using SportsGurukul.Application.Common.Interfaces.AI;
using SportsGurukul.Application.Common.Interfaces.AI.Services;
using SportsGurukul.Application.Common.Models;
using SportsGurukul.Domain.Enums.AI;

namespace SportsGurukul.Application.Features.AIManagement.Services;

public class PromptRenderer : IPromptRenderer
{
    private readonly IPromptRepository _promptRepository;
    private readonly ILogger<PromptRenderer> _logger;

    public PromptRenderer(
        IPromptRepository promptRepository,
        ILogger<PromptRenderer> logger)
    {
        _promptRepository = promptRepository;
        _logger = logger;
    }

    public Result<string> Render(string template, IReadOnlyDictionary<string, string> variables)
    {
        try
        {
            var rendered = template;
            foreach (var variable in variables)
            {
                rendered = rendered.Replace(
                    $"{{{{{variable.Key}}}}}",
                    variable.Value,
                    StringComparison.OrdinalIgnoreCase);
            }

            return Result<string>.Success(rendered);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Prompt rendering failed");
            return Result<string>.Failure("Prompt rendering failed");
        }
    }

    public async Task<Result<string>> ResolveAndRenderAsync(
        Guid assistantId,
        AIPromptType promptType,
        IReadOnlyDictionary<string, string> variables,
        CancellationToken cancellationToken = default)
    {
        var template = await _promptRepository.GetDefaultByAssistantAsync(assistantId, cancellationToken);
        template ??= (await _promptRepository.GetActiveByAssistantAsync(assistantId, cancellationToken))
            .FirstOrDefault(t => t.PromptType == promptType);

        if (template is null)
            return Result<string>.Failure("No active prompt template found for the assistant");

        return Render(template.TemplateText, variables);
    }
}
