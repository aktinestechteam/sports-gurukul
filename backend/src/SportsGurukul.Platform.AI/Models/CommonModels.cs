namespace SportsGurukul.Platform.AI.Models;

public class AgentPlatformException : Exception
{
    public string? Code { get; }

    public AgentPlatformException(string message, string? code = null, Exception? inner = null)
        : base(message, inner)
    {
        Code = code;
    }
}

public sealed class AgentNotFoundException : AgentPlatformException
{
    public AgentNotFoundException(string agentId)
        : base($"Agent '{agentId}' was not found.", "AGENT_NOT_FOUND")
    {
    }
}

public sealed class ToolNotFoundException : AgentPlatformException
{
    public ToolNotFoundException(string toolName)
        : base($"Tool '{toolName}' was not found.", "TOOL_NOT_FOUND")
    {
    }
}

public sealed class WorkflowNotFoundException : AgentPlatformException
{
    public WorkflowNotFoundException(string workflowName)
        : base($"Workflow '{workflowName}' was not found.", "WORKFLOW_NOT_FOUND")
    {
    }
}

public sealed class WorkflowExecutionNotFoundException : AgentPlatformException
{
    public WorkflowExecutionNotFoundException(Guid executionId)
        : base($"Workflow execution '{executionId}' was not found.", "WORKFLOW_EXECUTION_NOT_FOUND")
    {
    }
}

public sealed class ToolAuthorizationException : AgentPlatformException
{
    public ToolAuthorizationException(string toolName, string reason)
        : base($"Tool '{toolName}' authorization denied: {reason}", "TOOL_AUTHORIZATION_DENIED")
    {
    }
}

public sealed class ApprovalRequiredException : AgentPlatformException
{
    public ApprovalRequiredException(Guid approvalRequestId)
        : base($"Approval required. Request id: {approvalRequestId}.", "APPROVAL_REQUIRED")
    {
        ApprovalRequestId = approvalRequestId;
    }

    public Guid ApprovalRequestId { get; }
}
