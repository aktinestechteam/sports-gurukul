using Microsoft.Extensions.Logging;
using SportsGurukul.Platform.AI.Interfaces.Model;
using SportsGurukul.Platform.AI.Interfaces.MultiAgent;
using SportsGurukul.Platform.AI.Models;

namespace SportsGurukul.Platform.AI.MultiAgent;

public class WorkerAgent : IWorkerAgent
{
    private readonly ILanguageModel _model;
    private readonly string? _systemPrompt;
    private readonly ILogger<WorkerAgent> _logger;

    public WorkerAgent(
        string name,
        IReadOnlyList<string> capabilities,
        ILanguageModel model,
        string? systemPrompt = null,
        ILogger<WorkerAgent>? logger = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name;
        Capabilities = capabilities ?? [];
        _model = model;
        _systemPrompt = systemPrompt;
        _logger = logger ?? Microsoft.Extensions.Logging.Abstractions.NullLogger<WorkerAgent>.Instance;
    }

    public string Name { get; }

    public IReadOnlyList<string> Capabilities { get; }

    public async Task<DelegatedTaskResult> ExecuteAsync(DelegatedTask task, CancellationToken cancellationToken = default)
    {
        if (task.AssignedAgentId is not null && !task.AssignedAgentId.Equals(Name, StringComparison.OrdinalIgnoreCase))
        {
            return new DelegatedTaskResult
            {
                TaskId = task.TaskId,
                Goal = task.Goal,
                AgentId = Name,
                Succeeded = false,
                Error = $"Task was assigned to '{task.AssignedAgentId}', not '{Name}'."
            };
        }

        try
        {
            var messages = new List<ModelMessage>();
            if (!string.IsNullOrWhiteSpace(_systemPrompt))
            {
                messages.Add(ModelMessage.System(_systemPrompt));
            }

            messages.Add(ModelMessage.User($"Goal: {task.Goal}{Environment.NewLine}Input: {task.Input}"));

            var response = await _model.GenerateAsync(messages, null, cancellationToken);

            return new DelegatedTaskResult
            {
                TaskId = task.TaskId,
                Goal = task.Goal,
                AgentId = Name,
                Succeeded = response is not null && !string.IsNullOrWhiteSpace(response.Content),
                Answer = response?.Content,
                Error = string.IsNullOrWhiteSpace(response?.Content) ? "Model returned an empty response." : null,
                CompletedAt = DateTime.UtcNow
            };
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Worker agent '{Name}' failed task '{TaskId}'", Name, task.TaskId);
            return new DelegatedTaskResult
            {
                TaskId = task.TaskId,
                Goal = task.Goal,
                AgentId = Name,
                Succeeded = false,
                Error = ex.Message,
                CompletedAt = DateTime.UtcNow
            };
        }
    }
}
