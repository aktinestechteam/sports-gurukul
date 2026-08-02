namespace SportsGurukul.Platform.AI;

public class AIPlatformOptions
{
    public int MaxAgentIterations { get; set; } = 10;
    public int MaxToolCallsPerRun { get; set; } = 50;
    public bool EnableReflection { get; set; } = true;
    public int ReflectionFrequency { get; set; } = 3;
    public bool EnableSelfEvaluation { get; set; } = true;
    public int DefaultToolTimeoutSeconds { get; set; } = 30;
    public int ToolRetryMax { get; set; } = 1;
    public bool EnableMetrics { get; set; } = true;
    public bool EnableAuditLogging { get; set; } = true;
    public bool EnableStreaming { get; set; } = true;
    public bool RunWorkflowStepsInParallel { get; set; }
    public int WorkflowRetryDelaySeconds { get; set; } = 1;
    public int ApprovalDefaultTimeoutMinutes { get; set; } = 60;
    public int ApprovalEscalationThresholdMinutes { get; set; } = 30;
    public string DefaultLanguageModelProvider { get; set; } = "stub";
    public string DefaultLanguageModelName { get; set; } = "stub-model";
}
