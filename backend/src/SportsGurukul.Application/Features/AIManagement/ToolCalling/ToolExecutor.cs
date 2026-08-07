using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SportsGurukul.Application.Features.AIManagement.ToolCalling;

public class ToolExecutor : IToolExecutor
{
    private readonly IToolResolver _resolver;
    private readonly IToolAuthorizationService _authorizationService;
    private readonly ILogger<ToolExecutor> _logger;

    public ToolExecutor(
        IToolResolver resolver,
        IToolAuthorizationService authorizationService,
        ILogger<ToolExecutor> logger)
    {
        _resolver = resolver;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<ToolCallResult> ExecuteAsync(
        string toolName,
        ToolCallRequest request,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        var descriptor = await _resolver.ResolveAsync(toolName, cancellationToken);
        if (descriptor is null)
            return ToolCallResult.Failure($"Tool '{toolName}' is not registered", stopwatch.ElapsedMilliseconds);

        var authorization = await _authorizationService.AuthorizeAsync(descriptor, request.Context, cancellationToken);
        if (!authorization.IsSuccess)
        {
            _logger.LogWarning("Tool '{ToolName}' execution denied: {Reason}", toolName, authorization.Error);
            return descriptor.RequiresApproval
                ? ToolCallResult.ApprovalRequired(stopwatch.ElapsedMilliseconds)
                : ToolCallResult.Failure(authorization.Error ?? "Tool execution denied", stopwatch.ElapsedMilliseconds);
        }

        if (descriptor.Executor is null)
            return ToolCallResult.Failure($"Tool '{toolName}' has no registered executor", stopwatch.ElapsedMilliseconds);

        try
        {
            var result = await descriptor.Executor(request, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tool '{ToolName}' execution failed", toolName);
            return ToolCallResult.Failure(ex.Message, stopwatch.ElapsedMilliseconds);
        }
    }
}
